import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import { setupRouterGuards } from './guards'

const routes = [
    {
        path: '/login',
        name: 'Login',
        component: Login,
        meta: { guest: true }
    },
    {
        path: '/',
        name: 'Dashboard',
        component: () => import('../views/Dashboard.vue'),
        meta: { requiresAuth: true }
    },
    {
        path: '/companies',
        name: 'Companies',
        component: () => import('../views/Companies.vue'),
        meta: { requiresAuth: true, roles: ['Admin'] }
    },
    {
        path: '/contracts',
        name: 'Contracts',
        component: () => import('../views/Contracts.vue'),
        meta: { requiresAuth: true, roles: ['Admin'] }
    },
    {
        path: '/groups',
        name: 'Groups',
        component: () => import('../views/Groups.vue'),
        meta: { requiresAuth: true, roles: ['Admin'] }
    },
    {
        path: '/staff',
        name: 'Staff',
        component: () => import('../views/Staff.vue'),
        meta: { requiresAuth: true, roles: ['Admin'] }
    },
    {
        path: '/supplies',
        name: 'Supplies',
        component: () => import('../views/Supplies.vue'),
        meta: { requiresAuth: true, roles: ['Admin'] }
    },
    {
        path: '/reports',
        name: 'Reports',
        component: () => import('../views/Reports.vue'),
        meta: { requiresAuth: true } // All authenticated users can view reports
    },
    {
        path: '/patients',
        name: 'Patients',
        component: () => import('../views/Patients.vue'),
        meta: { requiresAuth: true, roles: ['Admin'] }
    },
    {
        path: '/users',
        name: 'Users',
        component: () => import('../views/Users.vue'),
        meta: { requiresAuth: true, roles: ['Admin'] }
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

// Apply navigation guards
setupRouterGuards(router)

export default router
