import { createWebHistory, createRouter } from 'vue-router'
import Layout from '@/layout'


export const constantRoutes = [
    {
        path: '/',
        component: () => import('@/views/login'),
        hidden: true
    },
    // {
    //     path: '',
    //     component: Layout,
    //     redirect: '/index',
    //     children: [
    //         {
    //             path: '/index',
    //             component: () => import('@/views/index'),
    //             name: 'Index',
    //             meta: { title: '首页', icon: 'index', affix: true, titleKey: 'menu.home' }
    //         }
    //     ]
    // },
    {
        path: '/user',
        component: Layout,
        hidden: true,
        redirect: 'noredirect',
        children: [
            {
                path: 'profile',
                component: () => import('@/views/system/profile/index'),
                name: 'Profile',
                meta: { title: '个人中心', icon: 'user', titleKey: 'menu.personalCenter' }
            }
        ]
    }
]

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
})

export default router
