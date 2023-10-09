import { createWebHashHistory, createRouter } from "vue-router";

// 公共路由
export const constantRoutes = [
    {
        path: '',
        component: () => import('@/layout/index.vue'),
        hidden: true,
    }
]

const router = createRouter({
    history: createWebHashHistory(),
    routes: constantRoutes,
    // scrollBehavior(to, from, savedPosition) {
    //     if (savedPosition) {
    //         return savedPosition
    //     } else {
    //         return { top: 0 }
    //     }
    // }
})

export default router
