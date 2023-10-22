<template>
    <el-config-provider :locale="zh">
        <router-view />
    </el-config-provider>
</template>

<script setup>
import zh from 'element-plus/dist/locale/zh-cn.mjs' // 中文语言
</script>
