<template>
    <starBackground></starBackground>
    <div class="login-wrap">
        <div class="login">
            <h3 class="title">{{ defaultSettings.title }}</h3>

            <div style="padding: 0 25px 5px 25px;text-align: center;">
                账号登陆
            </div>

            <el-form ref="loginRef" :model="loginForm" :rules="loginRules" class="login-form" v-show="loginType == 1">
                <el-form-item prop="username">
                    <el-input v-model="loginForm.username" type="text" auto-complete="off" placeholder="账号">
                        <template #prefix>
                            <svg-icon name="user" class="input-icon" />
                        </template>
                    </el-input>
                </el-form-item>
                <el-form-item prop="password">
                    <el-input v-model="loginForm.password" type="password" auto-complete="off" placeholder="密码"
                        @keyup.enter="handleLogin">
                        <template #prefix>
                            <svg-icon name="password" class="input-icon" />
                        </template>
                    </el-input>
                </el-form-item>
                <el-form-item prop="code" v-if="captchaOnOff != 'off'">
                    <el-input v-model="loginForm.code" auto-complete="off" placeholder="验证码" style="width: 63%"
                        @keyup.enter="handleLogin">
                        <template #prefix>
                            <svg-icon name="validCode" class="input-icon" />
                        </template>
                    </el-input>
                    <div class="login-code">
                        <el-image :src="codeUrl" @click="getCode" class="login-code-img" />
                    </div>
                </el-form-item>

                <el-form-item style="width: 100%" :style="{ 'margin-top': captchaOnOff == 'off' ? '40px' : '' }">
                    <el-button :loading="loading" size="default" round type="primary" style="width: 100%"
                        @click.prevent="handleLogin">
                        <span v-if="!loading">登录</span>
                        <span v-else>登 录 中...</span>
                    </el-button>
                </el-form-item>

                <div style="display: flex; justify-content: space-between; align-items: center">
                    <el-checkbox v-model="loginForm.rememberMe">记住密码</el-checkbox>
                    <span style="font-size: 12px">
                        <router-link class="link-type" :to="'/register'">立即注册</router-link>
                        <span @click="handleForgetPwd()" class="forget-pwd">忘记密码</span>
                    </span>
                </div>
            </el-form>
        </div>

        <div class="el-login-footer">
            <div v-html="defaultSettings.copyright"></div>
        </div>
    </div>
</template>
  
<script setup name="login">
import { getCodeImg } from '@/api/system/login'
import Cookies from 'js-cookie'
import { encrypt, decrypt } from '@/utils/jsencrypt'
import defaultSettings from '@/settings'
import starBackground from '@/views/components/starBackground.vue'
import useUserStore from '@/store/modules/user'

var visitorId = ''
const fpPromise = import('https://openfpcdn.io/fingerprintjs/v3').then((FingerprintJS) => FingerprintJS.load())

const userStore = useUserStore()
const router = useRouter()
const route = useRoute()
const { proxy } = getCurrentInstance()

const loginForm = ref({
    username: '',
    password: '',
    rememberMe: false,
    code: '',
    uuid: ''
})
const host = window.location.host
const loginRules = {
    username: [{ required: true, trigger: 'blur', message: '请输入您的账号' }],
    password: [{ required: true, trigger: 'blur', message: '请输入您的密码' }],
    code: [{ required: true, trigger: 'change', message: '请输入验证码' }]
}
const loginType = ref(1)
const codeUrl = ref('')
const loading = ref(false)
// 验证码开关
const captchaOnOff = ref('')
// 注册开关
const register = ref(false)
const redirect = ref()
redirect.value = route.query.redirect
// Get the visitor identifier when you need it.
fpPromise
    .then((fp) => fp.get())
    .then((result) => {
        // This is the visitor identifier:
        visitorId = result.visitorId
        userStore.setClientId(visitorId)
    })
function handleLogin() {
    proxy.$refs.loginRef.validate((valid) => {
        if (valid) {
            loading.value = true
            // 勾选了需要记住密码设置在cookie中设置记住用户名和密码
            if (loginForm.value.rememberMe) {
                Cookies.set('username', loginForm.value.username, { expires: 30, path: host })
                Cookies.set('password', encrypt(loginForm.value.password), { expires: 30, path: host })
                Cookies.set('rememberMe', loginForm.value.rememberMe, { expires: 30, path: host })
            } else {
                // 否则移除
                Cookies.remove('username')
                Cookies.remove('password')
                Cookies.remove('rememberMe')
            }
            // 调用action的登录方法
            userStore
                .login(loginForm.value)
                .then(() => {
                    proxy.$modal.msgSuccess(proxy.$t('login.loginSuccess'))
                    router.push({ path: redirect.value || '/' })
                })
                .catch((error) => {
                    console.error(error)
                    proxy.$modal.msgError(error.msg)
                    loading.value = false
                    // 重新获取验证码
                    if (captchaOnOff.value) {
                        getCode()
                    }
                })
        }
    })
}

function getCode() {
    getCodeImg().then((res) => {
        codeUrl.value = 'data:image/gif;base64,' + res.data.img
        loginForm.value.uuid = res.data.uuid
        captchaOnOff.value = res.data.captchaOff
    })
}

function getCookie() {
    const username = Cookies.get('username')
    const password = Cookies.get('password')
    const rememberMe = Cookies.get('rememberMe')
    loginForm.value = {
        username: username === undefined ? loginForm.value.username : username,
        password: password === undefined ? loginForm.value.password : decrypt(password),
        rememberMe: rememberMe === undefined ? false : Boolean(rememberMe)
    }
}
function handleForgetPwd() {
    proxy.$modal.msg('请联系管理员')
}

const interval = ref(null)
function handleShowQrLogin() {
    nextTick(() => {
        generateCode()
    })
}

function getUuid() {
    var temp_url = URL.createObjectURL(new Blob())
    var uuid = temp_url.toString().replace('-', '') // blob:https://xxx.com/b250d159-e1b6-4a87-9002-885d90033be3
    URL.revokeObjectURL(temp_url)
    return uuid.substr(uuid.lastIndexOf('/') + 1)
}
function handleLoginType(t) {
    const val = t.paneName

    if (val == 3) {
        handleShowQrLogin()
    } else {
        clearQr()
    }
}
getCode()
getCookie()
</script>
  
<style lang="scss" scoped>
@import '@/assets/styles/login.scss';

.forget-pwd {
    color: #ccc;
    margin-left: 10px;
    cursor: pointer;
    border-left: 1px solid;
    padding-left: 10px;
}

.qrCode {
    width: 160px;
    height: 160px;
    line-height: 160px;
}
</style>
  