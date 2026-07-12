<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span class="title">审计日志</span>
        </div>
      </template>

      <el-form :model="filterForm" inline class="filter-form">
        <el-form-item label="操作类型">
          <el-select v-model="filterForm.action" placeholder="全部" clearable style="width: 140px">
            <el-option label="登录" value="login" />
            <el-option label="登出" value="logout" />
            <el-option label="创建" value="create" />
            <el-option label="更新" value="update" />
            <el-option label="删除" value="delete" />
            <el-option label="查看" value="view" />
            <el-option label="审批" value="approve" />
          </el-select>
        </el-form-item>
        <el-form-item label="资源类型">
          <el-select v-model="filterForm.resource_type" placeholder="全部" clearable style="width: 140px">
            <el-option label="资产" value="asset" />
            <el-option label="凭据" value="credential" />
            <el-option label="审批" value="approval" />
            <el-option label="用户" value="user" />
            <el-option label="系统" value="system" />
          </el-select>
        </el-form-item>
        <el-form-item label="关键字">
          <el-input v-model="filterForm.keyword" placeholder="用户/对象" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon> 查询
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <el-table :data="logList" v-loading="loading" stripe border>
        <el-table-column prop="created_at" label="时间" width="160" />
        <el-table-column prop="username" label="用户" width="120" />
        <el-table-column prop="action" label="操作" width="100">
          <template #default="scope">
            <el-tag size="small" :type="actionType(scope.row.action)">{{ scope.row.action }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="resource_type" label="资源类型" width="100" />
        <el-table-column prop="object" label="对象" width="180" show-overflow-tooltip />
        <el-table-column prop="result" label="结果" width="90">
          <template #default="scope">
            <el-tag :type="scope.row.result === 'success' ? 'success' : 'danger'" size="small">
              {{ scope.row.result === 'success' ? '成功' : '失败' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ip_address" label="IP地址" width="130" />
        <el-table-column prop="detail" label="详情" show-overflow-tooltip />
      </el-table>

      <div class="pagination-wrapper">
        <el-pagination
          v-model:current-page="pagination.page"
          v-model:page-size="pagination.pageSize"
          :total="pagination.total"
          layout="total, sizes, prev, pager, next"
          :page-sizes="[10, 20, 50]"
          @size-change="fetchLogs"
          @current-change="fetchLogs"
        />
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { auditApi } from '../api'

const loading = ref(false)
const logList = ref([])
const pagination = reactive({
  page: 1,
  pageSize: 20,
  total: 0
})

const filterForm = reactive({
  action: '',
  resource_type: '',
  keyword: ''
})

const actionTypeMap = {
  login: 'primary',
  logout: 'info',
  create: 'success',
  update: 'warning',
  delete: 'danger',
  view: '',
  approve: 'success'
}

const actionType = (action) => actionTypeMap[action] || ''

const fetchLogs = async () => {
  loading.value = true
  try {
    const res = await auditApi.list({
      page: pagination.page,
      pageSize: pagination.pageSize,
      action: filterForm.action,
      resource_type: filterForm.resource_type,
      keyword: filterForm.keyword
    })
    logList.value = res.data?.list || []
    pagination.total = res.data?.total || 0
  } catch (e) {
    ElMessage.error('获取审计日志失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  pagination.page = 1
  fetchLogs()
}

const handleReset = () => {
  filterForm.action = ''
  filterForm.resource_type = ''
  filterForm.keyword = ''
  pagination.page = 1
  fetchLogs()
}

onMounted(fetchLogs)
</script>

<style scoped>
.page-container {
  padding: 0;
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

.filter-form {
  margin-bottom: 10px;
}

.pagination-wrapper {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}
</style>
