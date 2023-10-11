<template>
    <div class="navbar">
        <hamburger/>
    </div>
</template>

<script setup lang="ts">
import hamburger from '@/components/Hamburger/index.vue'

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
    // box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);

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
}</style>