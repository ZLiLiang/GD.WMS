<template>
    <el-container class="app-layout">
        <sidebar/>
        <el-container class="main-container flex-center">
            <el-header>
                <navbar/>
            </el-header>
            <el-main class="app-main">
                <svg-icon :name="name" />
                <el-icon><setting /></el-icon>
            </el-main>
            <el-footer>Footer</el-footer>
        </el-container>
    </el-container>
</template>
  

<script setup lang="ts">
import sidebar from './components/Sidebar/index.vue'
import navbar from './components/Navbar.vue'

const name="setting"

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
}</style>