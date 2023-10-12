<template>
    <template v-if="!item.hidden">
        <template
            v-if="hasOneShowingChild(item.children, item) && (!onlyOneChild.children || onlyOneChild.noShowingChildren) && !item.alwaysShow">
            <appLink v-if="onlyOneChild.meta" :to="resolvePath(onlyOneChild.path, onlyOneChild.query)">
                <el-menu-item :index="resolvePath(onlyOneChild.path)">
                    <svg-icon :name="onlyOneChild.meta.icon || (item.meta && item.meta.icon)" />
                    <span v-if="props.isCollapse && !onlyOneChild.meta.icon">{{ hasTitle2(onlyOneChild.meta.title) }}</span>
                    <template #title>
                        <span v-if="onlyOneChild.meta.title">{{ onlyOneChild.meta.title }}</span>
                        <svg-icon name="new" color="#fff" style="width: 50px; height: 25px"
                            v-if="onlyOneChild.meta.title && onlyOneChild.meta.isNew == 1 && defaultSettings.menuShowNew" />
                    </template>
                </el-menu-item>
            </appLink>
        </template>

        <el-sub-menu v-else ref="subMenu" :index="resolvePath(item.path)">
            <template #title>
                <svg-icon :name="item.meta && item.meta.icon" />
                <span v-if="item.meta && item.meta.title">{{ item.meta.title }}</span>
                <svg-icon name="new" color="#fff" style="width: 50px; height: 25px"
                    v-if="item.meta.title && item.meta.isNew == 1&& defaultSettings.menuShowNew" />
            </template>
            <sidebar-item v-for="child in item.children" :key="child.path" :is-nest="true" :item="child"
                :base-path="resolvePath(child.path)" />
        </el-sub-menu>
    </template>
</template>

<script setup lang="ts">
import { getNormalPath } from '@/utils/ruoyi'
import appLink from './Link.vue'
import defaultSettings from '@/settings'
const props = defineProps({
    // route object
    item: {
        type: Object,
        required: true
    },
    isNest: {
        type: Boolean,
        default: false
    },
    basePath: {
        type: String,
        default: ''
    },
    isCollapse: {
        type: Boolean,
        default: false
    }
})

const onlyOneChild = ref<any>({})

function hasOneShowingChild(children: Array<any>, parent: any) {
    if (!children) {
        children = []
    }
    const showingChildren = children.filter((item: any) => {
        if (item.hidden) {
            return false
        } else {
            // Temp set(will be used if only has one showing child)
            onlyOneChild.value = item
            return true
        }
    })

    // When there is only one child router, the child router is displayed by default
    if (showingChildren.length === 1) {
        return true
    }

    // Show parent if there are no child router to display
    if (showingChildren.length === 0) {
        onlyOneChild.value = { ...parent, path: '', noShowingChildren: true }
        return true
    }

    return false
}

function resolvePath(routePath: string, routeQuery?: string) {
    if (routeQuery) {
        let query = JSON.parse(routeQuery)
        return {
            path: getNormalPath(props.basePath + '/' + routePath),
            query: query
        }
    }
    return getNormalPath(props.basePath + '/' + routePath)
}

function hasTitle2(title: string) {
    console.log(title)
    if (title.length >= 1) {
        return title.charAt(0) + '...'
    } else {
        return ''
    }
}

</script>
