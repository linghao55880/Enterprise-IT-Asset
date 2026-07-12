import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import AppLayout from '../components/AppLayout.vue'
import Dashboard from '../views/Dashboard.vue'
import Assets from '../views/Assets.vue'
import Credentials from '../views/Credentials.vue'
import Approvals from '../views/Approvals.vue'
import Audit from '../views/Audit.vue'
import Users from '../views/Users.vue'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: { public: true }
  },
  {
    path: '/',
    component: AppLayout,
    redirect: '/dashboard',
    children: [
      { path: 'dashboard', name: 'Dashboard', component: Dashboard },
      { path: 'assets', name: 'Assets', component: Assets },
      { path: 'credentials', name: 'Credentials', component: Credentials },
      { path: 'approvals', name: 'Approvals', component: Approvals },
      { path: 'audit', name: 'Audit', component: Audit },
      { path: 'users', name: 'Users', component: Users }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const userStr = localStorage.getItem('user')
  const user = userStr ? JSON.parse(userStr) : null

  if (!to.meta.public && !user) {
    next('/login')
  } else {
    next()
  }
})

export default router
