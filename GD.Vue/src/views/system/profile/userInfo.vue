<template>
    <el-form ref="userRef" :model="user" :rules="rules" label-width="130px" label-position="left">
        <el-form-item label="用户名" prop="nickName">
            <el-input v-model="user.nickName" maxlength="30" />
        </el-form-item>
        <el-form-item label="电话号码" prop="phonenumber">
            <el-input v-model="user.phonenumber" maxlength="11" />
        </el-form-item>
        <el-form-item label="电子邮箱" prop="email">
            <el-input v-model="user.email" maxlength="50" />
        </el-form-item>
        <el-form-item label="性别">
            <el-radio-group v-model="user.sex">
                <el-radio :label="0">男</el-radio>
                <el-radio :label="1">女</el-radio>
            </el-radio-group>
        </el-form-item>
        <div style="margin-left: 40px">
            <el-button type="danger" icon="Close" @click="close">取消</el-button>
            <el-button type="primary" icon="Check" @click="submit">保存</el-button>
        </div>
    </el-form>
</template>
  
<script setup>
// import { updateUserProfile } from '@/api/system/user'

const props = defineProps({
  user: {
    type: Object
  }
})

const { proxy } = getCurrentInstance()

const rules = ref({
  nickName: [{ required: true, message: '用户昵称不能为空', trigger: 'blur' }],
  email: [
    { required: true, message: '邮箱地址不能为空', trigger: 'blur' },
    {
      type: 'email',
      message: "'请输入正确的邮箱地址",
      trigger: ['blur', 'change']
    }
  ],
  phonenumber: [
    { required: true, message: '手机号码不能为空', trigger: 'blur' },
    {
      pattern: /^1[3|4|5|6|7|8|9][0-9]\d{8}$/,
      message: '请输入正确的手机号码',
      trigger: 'blur'
    }
  ]
})

/** 提交按钮 */
function submit() {
  proxy.$refs.userRef.validate((valid) => {
    if (valid) {
    //   updateUserProfile(props.user).then((response) => {
    //     proxy.$modal.msgSuccess('修改成功')
    //   })
    }
  })
}
/** 关闭按钮 */
function close() {
  proxy.$tab.closePage()
}
</script>