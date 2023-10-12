import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";

// 配置路由
export const constantRoutes: Array<RouteRecordRaw> = [
    {
        path: "/",
        component: () => import("@/layout/index.vue"),
    },
];
// 返回一个 router 实列，为函数，里面有配置项（对象） history
const router = createRouter({
    history: createWebHistory(),
    routes: constantRoutes,
    scrollBehavior(to, from, savedPosition) {
        if (savedPosition) {
            return savedPosition
        } else {
            return { top: 0 }
        }
    }
});

// 导出路由   然后去 main.ts 注册 router.ts
export default router