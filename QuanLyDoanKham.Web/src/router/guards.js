import { useAuthStore } from '../stores/auth'

export function setupRouterGuards(router) {
    router.beforeEach((to, from, next) => {
        const authStore = useAuthStore()

        // Check if route requires authentication
        if (to.meta.requiresAuth && !authStore.isAuthenticated) {
            return next('/login')
        }

        // Check if route requires specific role
        if (to.meta.roles && to.meta.roles.length > 0) {
            const userRole = authStore.role
            if (!to.meta.roles.includes(userRole)) {
                // Redirect to dashboard if user doesn't have required role
                return next('/')
            }
        }

        // If already logged in and trying to access login page, redirect to dashboard
        if (to.path === '/login' && authStore.isAuthenticated) {
            return next('/')
        }

        next()
    })
}
