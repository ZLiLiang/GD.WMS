<template>
    <div class="navbar desktop">
        <hamburger id="hamburger-container" :is-active="appStore.sidebar.opened" class="hamburger-container"
            @toggleClick="toggleSideBar" />
        <breadcrumb id="breadcrumb-container" class="breadcrumb-container" />

        <div class="right-menu">
            <screenfull title="全屏" class="right-menu-item" />
            <el-dropdown class="right-menu-item avatar-container" trigger="hover">
                <span class="avatar-wrapper">
                    <el-avatar :size="25" shape="circle" class="user-avatar" :src="userStore.avatar" />
                    <span class="name">{{ userStore.name }}</span>
                    <el-icon>
                        <ArrowDown />
                    </el-icon>
                </span>
                <template #dropdown>
                    <el-dropdown-menu>
                        <router-link to="/user/profile">
                            <el-dropdown-item>个人中心</el-dropdown-item>
                        </router-link>
                        <el-dropdown-item @click="logout">
                            <span>退出登陆</span>
                        </el-dropdown-item>
                    </el-dropdown-menu>
                </template>
            </el-dropdown>
        </div>

    </div>
</template>

<script setup>
import hamburger from '@/components/Hamburger/index.vue'
import breadcrumb from '@/components/Breadcrumb/index.vue'
import useAppStore from '@/store/modules/app'
import useUserStore from '@/store/modules/user'

const appStore = useAppStore()
const userStore = useUserStore()
const { proxy } = getCurrentInstance()

function toggleSideBar() {
    appStore.toggleSideBar()
}

function logout() {
    proxy.$modal
        .confirm("你確定要退出当前登录吗？")
        .then(() => {
              userStore.logOut().then(() => {
                location.href = '/index'
              })
        })
        .catch(() => { })
}
</script>

<style lang="scss" scoped>
.el-menu {
    // display: inline-table;
    border-bottom: none;

    .el-menu-item {
        vertical-align: center;
    }
}

.navbar {
    height: var(--base-header-height);
    line-height: var(--base-header-height);
    overflow: hidden;
    position: relative;
    background: var(--base-topBar-background);
    box-shadow: 0 5px 14px rgba(0, 21, 41, 0.08);

    .hamburger-container {
        line-height: var(--base-header-height);
        height: 100%;
        float: left;
        cursor: pointer;
        transition: background 0.3s;
        -webkit-tap-highlight-color: transparent;

        &:hover {
            background: rgba(0, 0, 0, 0.025);
        }
    }

    .breadcrumb-container {
        float: left;
    }

    .topmenu-container {
        position: absolute;
        left: 50px;
    }

    .errLog-container {
        display: inline-block;
        vertical-align: top;
    }

    .right-menu {
        display: flex;
        justify-content: flex-end;
        align-items: center;

        &:focus {
            outline: none;
        }

        .right-menu-item {
            padding: 0 8px;
            color: var(--base-topBar-color);
            vertical-align: text-bottom;
        }

        .avatar-container {
            .avatar-wrapper {
                display: flex;
                align-items: center;

                .user-avatar {
                    cursor: pointer;
                    width: 30px;
                    height: 30px;
                    border-radius: 50%;
                    vertical-align: middle;
                    margin-right: 5px;
                }

                .name {
                    font-size: 12px;
                }

                i {
                    cursor: pointer;
                    margin-left: 10px;
                }
            }
        }
    }
}
</style>