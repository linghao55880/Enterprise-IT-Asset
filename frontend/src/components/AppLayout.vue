<template>
  <el-container class="app-layout">
    <el-aside :width="isCollapse ? '64px' : '220px'" class="sidebar">
      <div class="logo">
        <el-icon :size="28"><Monitor /></el-icon>
        <span v-show="!isCollapse" class="logo-text">IT资产管理</span>
      </div>
      <el-menu
        :default-active="$route.path"
        :collapse="isCollapse"
        :collapse-transition="false"
        router
        class="sidebar-menu"
        background-color="#304156"
        text-color="#bfcbd9"
        active-text-color="#409EFF"
      >
        <el-menu-item index="/dashboard">
          <el-icon><Odometer /></el-icon>
          <template #title>仪表盘</template>
        </el-menu-item>

        <el-menu-item index="/assets">
          <el-icon><Box /></el-icon>
          <template #title>资产台账</template>
        </el-menu-item>

        <el-menu-item index="/credentials">
          <el-icon><Key /></el-icon>
          <template #title>凭据管理</template>
        </el-menu-item>

        <el-menu-item v-if="showApprovals" index="/approvals">
          <el-icon><DocumentChecked /></el-icon>
          <template #title>审批中心</template>
        </el-menu-item>

        <el-menu-item v-if="showAudit" index="/audit">
          <el-icon><Timer /></el-icon>
          <template #title>审计日志</template>
        </el-menu-item>

        <el-menu-item v-if="showUsers" index="/users">
          <el-icon><UserFilled /></el-icon>
          <template #title>系统管理</template>
        </el-menu-item>
      </el-menu>
    </el-aside>

    <el-container>
      <el-header class="app-header">
        <div class="header-left">
          <el-icon class="collapse-btn" @click="isCollapse = !isCollapse">
            <Fold v-if="!isCollapse" />
            <Expand v-else />
          </el-icon>
          <span class="system-name">IT资产管理平台</span>
        </div>
        <div class="header-right">
          <el-dropdown @command="handleCommand">
            <span class="user-info">
              <el-icon><User /></el-icon>
              {{ user?.username || '未知用户' }}
              <el-icon class="el-icon--right"><ArrowDown /></el-icon>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="profile">个人信息</el-dropdown-item>
                <el-dropdown-item divided command="logout">退出登录</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-header>

      <el-main class="app-main">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const isCollapse = ref(false)

const userStr = localStorage.getItem('user')
const user = ref(userStr ? JSON.parse(userStr) : null)

const role = computed(() => user.value?.role || '')

const showApprovals = computed(() => {
  return ['secmgr', 'depthead', 'member', 'admin'].includes(role.value)
})

const showAudit = computed(() => {
  return ['auditor', 'admin', 'secmgr'].includes(role.value)
})

const showUsers = computed(() => {
  return role.value === 'admin'
})

const handleCommand = (cmd) => {
  if (cmd === 'logout') {
    localStorage.removeItem('user')
    router.push('/login')
  }
}
</script>

<style scoped>
.app-layout {
  height: 100vh;
}

.sidebar {
  background-color: #304156;
  transition: width 0.3s;
}

.logo {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  font-size: 18px;
  font-weight: bold;
  border-bottom: 1px solid #1f2d3d;
}

.logo-text {
  margin-left: 10px;
}

.sidebar-menu {
  border-right: none;
}

.app-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: #fff;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
}

.header-left {
  display: flex;
  align-items: center;
}

.collapse-btn {
  font-size: 20px;
  cursor: pointer;
  margin-right: 15px;
  color: #606266;
}

.system-name {
  font-size: 18px;
  font-weight: 600;
  color: #303133;
}

.header-right {
  display: flex;
  align-items: center;
}

.user-info {
  cursor: pointer;
  display: flex;
  align-items: center;
  color: #606266;
  font-size: 14px;
}

.user-info .el-icon {
  margin-right: 6px;
}

.app-main {
  background-color: #f0f2f5;
  padding: 20px;
  overflow-y: auto;
}
</style>
