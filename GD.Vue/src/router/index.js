import { createWebHistory, createRouter } from "vue-router";

export const constantRoutes = [
    {
        path: '/',
        component:() => import('@/layout/index'),
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes: constantRoutes,
    scrollBehavior(to, from, savePosition) {
        if (savePosition) {
            return savePosition
        } else {
            return { top: 0 }
        }
    }
})

export default router
