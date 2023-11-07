import { createWebHistory, createRouter } from 'vue-router'
import Layout from '@/layout'


export const constantRoutes = [
    {
        path: '/redirect',
        component: Layout,
        hidden: true,
        children: [
            {
                path: '/redirect/:path(.*)',
                component: () => import('@/views/redirect/index.vue')
            }
        ]
    },
    // {
    //     path: '/login_v2',
    //     component: () => import('@/views/login_v2'),
    //     hidden: true
    // },
    {
        path: '/login',
        component: () => import('@/views/login'),
        hidden: true
    },
    {
        path: '/sociallogin',
        component: () => import('@/views/socialLogin'),
        hidden: true
    },
    {
        path: '/register',
        component: () => import('@/views/register'),
        hidden: true
    },
    {
        path: '/:pathMatch(.*)*',
        component: () => import('@/views/error/404'),
        hidden: true
    },
    {
        path: '/401',
        component: () => import('@/views/error/401'),
        hidden: true
    },
    {
        path: '',
        component: Layout,
        redirect: '/index',
        children: [
            {
                path: '/index',
                component: () => import('@/views/index'),
                name: 'Index',
                meta: { title: '首页', icon: 'index', affix: true, titleKey: 'menu.home' }
            }
        ]
    },
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
    history: createWebHistory('/'),
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
