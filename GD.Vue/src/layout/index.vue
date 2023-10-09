<template>
    <el-container class="app-layout">
        <div>666</div>
    </el-container>
</template>

<script setup>

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
  // position: relative;
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
  /* 50= navbar  50  */
  // min-height: calc(100vh - 50px);
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
  // --el-header-height: 50px !important;
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
  // .app-main {
  //   min-height: calc(100vh - 84px - var(--base-footer-height));
  // }
  .el-header {
    --el-header-height: var(--el-header-height) + var(--el-tags-height) !important;
  }
}
</style>