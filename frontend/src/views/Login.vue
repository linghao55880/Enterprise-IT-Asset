<template>
  <div class="login-page">
    <el-card class="login-card" shadow="always">
      <template #header>
        <div class="login-header">
          <el-icon :size="40" color="#409EFF"><Monitor /></el-icon>
          <h2>IT资产管理平台</h2>
          <p>安全 · 高效 · 可控</p>
        </div>
      </template>

      <el-form :model="form" :rules="rules" ref="formRef" label-position="top">
        <el-form-item label="选择用户（演示用）">
          <el-select v-model="selectedUser" placeholder="请选择预置用户" @change="onUserChange" style="width: 100%">
            <el-option
              v-for="u in presetUsers"
              :key="u.username"
              :label="`${u.username} - ${u.displayName} (${u.roleName})`"
              :value="u.username"
            />
          </el-select>
        </el-form-item>

        <el-form-item label="用户名" prop="username">
          <el-input v-model="form.username" placeholder="请输入用户名" prefix-icon="User" />
        </el-form-item>

        <el-form-item label="密码" prop="password">
          <el-input v-model="form.password" type="password" placeholder="请输入密码" prefix-icon="Lock" show-password @keyup.enter="handleLogin" />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" :loading="loading" style="width: 100%" size="large" @click="handleLogin">
            登录
          </el-button>
        </el-form-item>
      </el-form>

      <div class="preset-hint">
        <el-divider>演示账号</el-divider>
        <el-tag v-for="u in presetUsers" :key="u.username" size="small" class="preset-tag">{{ u.username }}</el-tag>
        <div class="hint-text">密码均为 <el-tag type="danger" size="small">123456</el-tag></div>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { authApi } from '../api'

const router = useRouter()
const formRef = ref()
const loading = ref(false)
const selectedUser = ref('')

const presetUsers = [
  { username: 'admin', displayName: '系统管理员', role: 'admin', roleName: '超级管理员' },
  { username: 'assetmgr', displayName: '资产管理员', role: 'assetmgr', roleName: '资产管理员' },
  { username: 'secmgr', displayName: '安全管理员', role: 'secmgr', roleName: '安全管理员' },
  { username: 'auditor', displayName: '审计员', role: 'auditor', roleName: '审计员' },
  { username: 'depthead', displayName: '部门负责人', role: 'depthead', roleName: '部门负责人' },
  { username: 'member1', displayName: '普通成员1', role: 'member', roleName: '普通成员' },
  { username: 'member2', displayName: '普通成员2', role: 'member', roleName: '普通成员' }
]

const form = reactive({
  username: '',
  password: ''
})

const rules = {
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }]
}

const onUserChange = (val) => {
  const u = presetUsers.find(item => item.username === val)
  if (u) {
    form.username = u.username
    form.password = '123456'
  }
}

const handleLogin = async () => {
  try {
    await formRef.value.validate()
    loading.value = true

    const res = await authApi.login({
      username: form.username,
      password: form.password
    })

    if (res.token && res.user) {
      const preset = presetUsers.find(u => u.username === form.username)
      const user = {
        ...res.user,
        token: res.token,
        role: preset ? preset.role : res.user.role,
        displayName: preset ? preset.displayName : res.user.displayName
      }
      localStorage.setItem('user', JSON.stringify(user))
      ElMessage.success('登录成功')
      router.push('/dashboard')
    } else {
      ElMessage.error(res.message || '登录失败')
    }
  } catch (err) {
    if (err?.response) {
      ElMessage.error(err.response.data?.message || '登录失败')
    } else {
      ElMessage.error('登录失败')
    }
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-page {
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-card {
  width: 420px;
  border-radius: 12px;
}

.login-header {
  text-align: center;
}

.login-header h2 {
  margin: 12px 0 4px;
  color: #303133;
}

.login-header p {
  margin: 0;
  color: #909399;
  font-size: 14px;
}

.preset-hint {
  text-align: center;
  margin-top: 10px;
}

.preset-tag {
  margin: 4px;
}

.hint-text {
  margin-top: 10px;
  color: #606266;
  font-size: 13px;
}
</style>
