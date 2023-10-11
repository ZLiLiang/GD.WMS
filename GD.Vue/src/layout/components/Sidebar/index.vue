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

<script setup lang="ts">
import logo from './Logo.vue'
import sidebarItem from './SidebarItem.vue'
import useAppStore from '@/store/modules/app'
import useSettingsStore from '@/store/modules/settings'

const route = useRoute()
const appStore = useAppStore()
const settingsStore = useSettingsStore()
const permissionStore = [
    {
        path:'/s',
        name:'system',
        meta:{
            icon: "system",
            isNew: 1,
            link: "",
            noCache: false,
            title: "系统管理"
        },
        // children:[
        //     {
        //         name:'user',
        //         path:'',
        //         meta:{
        //             icon: "user",
        //             isNew: 1,
        //             link: "",
        //             noCache: false,
        //             title: "用户管理"
        //         }
        //     },{
        //         name:'user',
        //         path:'',
        //         meta:{
        //             icon: "user",
        //             isNew: 1,
        //             link: "",
        //             noCache: false,
        //             title: "测试管理"
        //         }
        //     }
        // ]
    },{
        path:'/a',
        name:'system',
        meta:{
            icon: "system",
            isNew: 1,
            link: "",
            noCache: false,
            title: "系统管理"
        },
        children:[
            {
                name:'user',
                path:'',
                meta:{
                    icon: "user",
                    isNew: 1,
                    link: "",
                    noCache: false,
                    title: "用户管理"
                }
            },{
                name:'user',
                path:'',
                meta:{
                    icon: "user",
                    isNew: 1,
                    link: "",
                    noCache: false,
                    title: "测试管理"
                }
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

