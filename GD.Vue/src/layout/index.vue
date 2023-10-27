<template>
    <el-container :class="classObj" class="app-layout" :style="{ '--current-color': theme }">
        <sidebar />
        <el-container class="main-container flex-center" :class="{ sidebarHide: sidebarAttr.hide }">
            <el-header :class="{ 'fixed-header': fixedHeader }">
                <navbar @setLayout="setLayout" />
            </el-header>
            <el-main class="app-main">
                <router-view v-slot="{ Component, route }">
                    <transition name="fade" mode="out-in">
                        <keep-alive>
                            <component v-if="!route.meta.link" :is="Component" :key="route.path" />
                        </keep-alive>
                    </transition>
                </router-view>
            </el-main>
        </el-container>
    </el-container>
</template>
  

<script setup>
import sidebar from './components/Sidebar/index.vue'
import navbar from './components/Navbar.vue'
import useAppStore from '@/store/modules/app'
import useSettingsStore from '@/store/modules/settings'

const settingsStore = useSettingsStore()
const theme = computed(() => settingsStore.theme)
const sidebarAttr = computed(() => useAppStore().sidebar)
const fixedHeader = computed(() => settingsStore.fixedHeader)

const classObj = computed(() => ({
    hideSidebar: !sidebarAttr.value.opened,
    openSidebar: sidebarAttr.value.opened
}))

const settingRef = ref(null)
function setLayout() {
    settingRef.value.openSetting()
}

</script>

<style lang="scss">
@import '@/assets/styles/mixin.scss';

.main-container {
    min-height: 100%;
    width: 100%;
    flex-direction: column;
    position: relative;
}

.app-layout {
    @include clearfix;
    height: 100%;
    width: 100%;
    display: flex;
    flex-direction: row;
    flex: 1;

    &.mobile.openSidebar {
        position: fixed;
        top: 0;
    }
}

// 固定header
.fixed-header {
    position: sticky;
    position: -webkit-sticky;
    z-index: 9;
}

.mobile .fixed-header {
    width: 100%;
}

.app-main {
    width: 100%;
    position: relative;
    height: 100%;
    overflow-x: hidden;
}

.sidebar-mobile {
    .el-drawer__body {
        padding: 0;
    }

    @media screen and (max-width: 700px) {
        .el-drawer {
            width: var(--base-sidebar-width) !important;
        }
    }
}

.el-header {
    --el-header-padding: 0 0px !important;
}

.el-footer {
    --el-footer-height: var(--base-footer-height);
    line-height: var(--base-footer-height);
    text-align: center;
    color: #ccc;
    font-size: 14px;
    border-top: 1px solid #e7eaec;
    letter-spacing: 0.1rem;
}

.hasTagsView {
    .el-header {
        --el-header-height: var(--el-header-height) + var(--el-tags-height) !important;
    }
}
</style>