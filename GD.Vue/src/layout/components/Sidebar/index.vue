<template>
    <el-aside :data-theme="sideTheme" class="sidebar" :class="{ 'has-logo': showLogo }">
        <logo v-if="showLogo" :collapse="isCollapse" />

        <el-scrollbar wrap-class="scrollbar-wrapper">
            <el-menu :default-active="activeMenu" :collapse="isCollapse" :unique-opened="true" :active-text-color="theme"
                :collapse-transition="false" background-color="transparent" mode="vertical">
                <sidebarItem v-for="(route, index) in sidebarRouters" :key="route.path + index" :item="route"
                    :base-path="route.path" :isCollapse="isCollapse" />
            </el-menu>
        </el-scrollbar>
    </el-aside>
</template>

<script setup>
import logo from './Logo.vue'
import sidebarItem from './SidebarItem.vue'
import useAppStore from '@/store/modules/app'
import useSettingsStore from '@/store/modules/settings'

const route = useRoute()
const appStore = useAppStore()
const settingsStore = useSettingsStore()
const permissionStore = [
    {
        path: "",
        component: "Index",
        children: [
            {
                component: "",
                name: "Index",
                path: "/index",
                meta: {
                    icon: "index",
                    title: "首页"
                }
            }
        ]
    },
    {
        alwaysShow: false,
        hidden: false,
        name: "",
        path: "",
        meta: null,
        children: [
            {
                alwaysShow: false,
                hidden: false,
                name: "dashboard",
                path: "dashboard",
                meta: {
                    icon: "dashboard",
                    title: "控制台",
                    isNew: 1,
                    link: "",
                    noCache: false
                },
            }
        ]
    },
    {
        alwaysShow: true,
        hidden: false,
        name: "system",
        path: "/system",
        meta: {
            icon: "system",
            title: "系统管理",
            isNew: 1,
            link: "",
            noCache: false
        },
        children: [
            {
                alwaysShow: false,
                hidden: false,
                name: "user",
                path: "user",
                meta: {
                    icon: "user",
                    title: "用户管理",
                    isNew: 1,
                    link: "",
                    noCache: false
                },
            },
            {
                alwaysShow: false,
                hidden: false,
                name: "role",
                path: "role",
                meta: {
                    icon: "peoples",
                    title: "角色管理",
                    isNew: 1,
                    link: "",
                    noCache: false
                },
            }
        ]
    }
]

const sidebarRouters = computed(() => permissionStore)
const showLogo = computed(() => settingsStore.sidebarLogo)
const sideTheme = computed(() => settingsStore.sideTheme)
const theme = computed(() => settingsStore.theme)
const isCollapse = computed(() => !appStore.sidebar.opened)
const activeMenu = computed(() => {
    const { meta, path } = route
    // if set path, the sidebar will highlight the path you set
    if (meta.activeMenu) {
        return meta.activeMenu
    }
    return path
})
</script>

