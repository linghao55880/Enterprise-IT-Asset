<template>
  <div class="page-container">
    <el-row :gutter="20">
      <el-col :xs="24" :sm="8" :md="6" :lg="5">
        <el-card shadow="never" class="group-card">
          <template #header>
            <div class="card-header">
              <span>分组</span>
            </div>
          </template>
          <el-menu :default-active="activeGroup" @select="handleGroupSelect" class="group-menu">
            <el-menu-item index="all">
              <el-icon><Folder /></el-icon>
              <span>全部</span>
            </el-menu-item>
            <el-menu-item v-for="g in groups" :key="g.value" :index="g.value">
              <el-icon><FolderOpened /></el-icon>
              <span>{{ g.label }}</span>
            </el-menu-item>
          </el-menu>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="16" :md="18" :lg="19">
        <el-card shadow="never">
          <template #header>
            <div class="card-header">
              <span class="title">凭据管理</span>
              <div class="header-actions">
                <el-input v-model="searchQuery" placeholder="搜索凭据名称" clearable style="width: 220px" @keyup.enter="handleSearch">
                  <template #append>
                    <el-button @click="handleSearch"><el-icon><Search /></el-icon></el-button>
                  </template>
                </el-input>
                <el-button v-if="canEdit" type="primary" @click="openDialog()">
                  <el-icon><Plus /></el-icon> 添加凭据
                </el-button>
              </div>
            </div>
          </template>

          <el-table :data="credentialList" v-loading="loading" stripe border>
            <el-table-column prop="name" label="名称" width="180" show-overflow-tooltip />
            <el-table-column prop="type" label="类型" width="120" />
            <el-table-column prop="asset_name" label="关联资产" width="160" show-overflow-tooltip />
            <el-table-column prop="username" label="用户名" width="140" />
            <el-table-column prop="password" label="密码" width="160">
              <template #default="scope">
                <div class="password-cell">
                  <span>{{ scope.row.showPassword ? scope.row.plainPassword : '********' }}</span>
                  <el-button v-if="canViewPassword(scope.row)" text type="primary" size="small" @click="requestShowPassword(scope.row)">
                    <el-icon><View /></el-icon>
                  </el-button>
                </div>
              </template>
            </el-table-column>
            <el-table-column prop="group" label="分组" width="120" />
            <el-table-column label="操作" width="220" fixed="right">
              <template #default="scope">
                <el-button text type="primary" size="small" @click="requestShowPassword(scope.row)">查看密码</el-button>
                <el-button v-if="canEdit" text type="primary" size="small" @click="openDialog(scope.row)">编辑</el-button>
                <el-button text type="primary" size="small" @click="viewDetail(scope.row)">详情</el-button>
                <el-button v-if="canEdit" text type="danger" size="small" @click="handleDelete(scope.row)">删除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <div class="pagination-wrapper">
            <el-pagination
              v-model:current-page="pagination.page"
              v-model:page-size="pagination.pageSize"
              :total="pagination.total"
              layout="total, sizes, prev, pager, next"
              :page-sizes="[10, 20, 50]"
              @size-change="fetchCredentials"
              @current-change="fetchCredentials"
            />
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑凭据' : '添加凭据'" width="600px" destroy-on-close>
      <el-form :model="form" :rules="formRules" ref="formRef" label-width="100px">
        <el-form-item label="名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入凭据名称" />
        </el-form-item>
        <el-form-item label="类型" prop="type">
          <el-select v-model="form.type" placeholder="请选择类型" style="width: 100%">
            <el-option label="系统账号" value="系统账号" />
            <el-option label="数据库" value="数据库" />
            <el-option label="SSH密钥" value="SSH密钥" />
            <el-option label="API密钥" value="API密钥" />
            <el-option label="其他" value="其他" />
          </el-select>
        </el-form-item>
        <el-form-item label="关联资产" prop="asset_id">
          <el-select v-model="form.asset_id" placeholder="请选择关联资产" filterable style="width: 100%">
            <el-option v-for="asset in assetOptions" :key="asset.id" :label="asset.name" :value="asset.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="用户名" prop="username">
          <el-input v-model="form.username" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item label="密码" prop="password">
          <el-input v-model="form.password" type="password" placeholder="请输入密码" show-password />
        </el-form-item>
        <el-form-item label="分组" prop="group">
          <el-select v-model="form.group" placeholder="请选择分组" style="width: 100%">
            <el-option v-for="g in groups" :key="g.value" :label="g.label" :value="g.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="form.remark" type="textarea" rows="2" placeholder="请输入备注" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitForm">确定</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="verifyVisible" title="二次验证" width="400px" :close-on-click-modal="false" @close="clearPasswordTimer">
      <p style="margin-bottom: 16px; color: #606266;">请输入验证密码以查看明文密码</p>
      <el-input v-model="verifyPassword" type="password" placeholder="请输入密码" show-password @keyup.enter="confirmShowPassword" />
      <template #footer>
        <el-button @click="verifyVisible = false">取消</el-button>
        <el-button type="primary" @click="confirmShowPassword">确认</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="detailVisible" title="凭据详情" width="500px">
      <el-descriptions :column="1" border>
        <el-descriptions-item label="名称">{{ detailData.name }}</el-descriptions-item>
        <el-descriptions-item label="类型">{{ detailData.type }}</el-descriptions-item>
        <el-descriptions-item label="关联资产">{{ detailData.asset_name }}</el-descriptions-item>
        <el-descriptions-item label="用户名">{{ detailData.username }}</el-descriptions-item>
        <el-descriptions-item label="分组">{{ detailData.group }}</el-descriptions-item>
        <el-descriptions-item label="备注">{{ detailData.remark }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">{{ detailData.created_at }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { credentialApi, assetApi } from '../api'

const loading = ref(false)
const searchQuery = ref('')
const activeGroup = ref('all')
const credentialList = ref([])
const pagination = reactive({
  page: 1,
  pageSize: 10,
  total: 0
})

const groups = [
  { label: '生产环境', value: 'production' },
  { label: '测试环境', value: 'testing' },
  { label: '网络设备', value: 'network' },
  { label: '云服务', value: 'cloud' },
  { label: '远程工具', value: 'remote' },
  { label: '其他', value: 'other' }
]

const userStr = localStorage.getItem('user')
const user = userStr ? JSON.parse(userStr) : {}
const canEdit = computed(() => ['admin', 'secmgr'].includes(user.role))

const canViewPassword = (row) => {
  if (['admin', 'secmgr'].includes(user.role)) return true
  return row.authorized === true
}

const handleGroupSelect = (index) => {
  activeGroup.value = index
  pagination.page = 1
  fetchCredentials()
}

const dialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref()
const assetOptions = ref([])
const form = reactive({
  id: null,
  name: '',
  type: '',
  asset_id: '',
  username: '',
  password: '',
  group: '',
  remark: ''
})

const formRules = {
  name: [{ required: true, message: '请输入名称', trigger: 'blur' }],
  type: [{ required: true, message: '请选择类型', trigger: 'change' }],
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
  group: [{ required: true, message: '请选择分组', trigger: 'change' }]
}

const detailVisible = ref(false)
const detailData = ref({})

const verifyVisible = ref(false)
const verifyPassword = ref('')
const currentCredential = ref(null)
let passwordTimer = null

const fetchCredentials = async () => {
  loading.value = true
  try {
    const params = { keyword: searchQuery.value }
    if (activeGroup.value !== 'all') {
      params.group = activeGroup.value
    }
    const res = await credentialApi.list(params)
    const list = (res.data?.list || []).map(item => {
      const asset = assetOptions.value.find(a => a.id === item.assetId)
      return {
        id: item.id,
        name: item.title,
        type: item.credentialType,
        asset_name: asset ? asset.name : (item.assetId || '-'),
        username: item.username,
        password: item.password,
        group: item.groupName,
        remark: item.notes,
        created_at: item.createdAt,
        showPassword: false,
        plainPassword: ''
      }
    })
    credentialList.value = list
    pagination.total = res.data?.total || 0
  } catch (e) {
    ElMessage.error('获取凭据列表失败')
  } finally {
    loading.value = false
  }
}

const fetchAssets = async () => {
  try {
    const res = await assetApi.list({ pageSize: 1000 })
    assetOptions.value = res.data?.list || []
  } catch (e) {
    console.error(e)
  }
}

const handleSearch = () => {
  pagination.page = 1
  fetchCredentials()
}

const openDialog = (row) => {
  isEdit.value = !!row
  if (row) {
    Object.assign(form, row)
  } else {
    Object.keys(form).forEach(key => {
      form[key] = ''
    })
    form.id = null
  }
  dialogVisible.value = true
}

const submitForm = async () => {
  try {
    await formRef.value.validate()
    const payload = { ...form }
    if (isEdit.value) {
      await credentialApi.update(form.id, payload)
      ElMessage.success('更新成功')
    } else {
      await credentialApi.create(payload)
      ElMessage.success('添加成功')
    }
    dialogVisible.value = false
    fetchCredentials()
  } catch (e) {
    if (e?.response) {
      ElMessage.error(e.response.data?.message || '操作失败')
    }
  }
}

const handleDelete = (row) => {
  ElMessageBox.confirm(`确定删除凭据 "${row.name}" 吗？`, '提示', {
    type: 'warning'
  }).then(async () => {
    await credentialApi.delete(row.id)
    ElMessage.success('删除成功')
    fetchCredentials()
  }).catch(() => {})
}

const viewDetail = (row) => {
  detailData.value = row
  detailVisible.value = true
}

const requestShowPassword = (row) => {
  currentCredential.value = row
  verifyPassword.value = ''
  verifyVisible.value = true
}

const confirmShowPassword = async () => {
  if (verifyPassword.value !== '123456') {
    ElMessage.error('验证密码错误')
    return
  }
  verifyVisible.value = false
  if (currentCredential.value) {
    try {
      const res = await credentialApi.reveal(currentCredential.value.id, { confirmPassword: verifyPassword.value })
      const plainPassword = res.password || 'N/A'
      const credId = currentCredential.value.id

      // 强制更新列表中的对应项以触发Vue响应式更新
      const idx = credentialList.value.findIndex(c => c.id === credId)
      if (idx !== -1) {
        credentialList.value[idx] = {
          ...credentialList.value[idx],
          plainPassword: plainPassword,
          showPassword: true
        }
      }

      if (passwordTimer) clearTimeout(passwordTimer)
      passwordTimer = setTimeout(() => {
        const curIdx = credentialList.value.findIndex(c => c.id === credId)
        if (curIdx !== -1) {
          credentialList.value[curIdx] = {
            ...credentialList.value[curIdx],
            showPassword: false,
            plainPassword: ''
          }
        }
      }, 30000)

      ElMessage.success('密码已显示，30秒后自动隐藏')
    } catch (e) {
      ElMessage.error('获取密码失败')
    }
  }
}

const clearPasswordTimer = () => {
  if (passwordTimer) {
    clearTimeout(passwordTimer)
    passwordTimer = null
  }
}

onMounted(async () => {
  await fetchAssets()
  await fetchCredentials()
})
</script>

<style scoped>
.page-container {
  padding: 0;
}

.group-card {
  margin-bottom: 20px;
}

.group-menu {
  border-right: none;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.title {
  font-size: 18px;
  font-weight: 600;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.password-cell {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.pagination-wrapper {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}
</style>
