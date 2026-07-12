<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span class="title">资产台账</span>
          <div class="header-actions">
            <el-input v-model="searchQuery" placeholder="搜索资产编号/名称/IP" clearable style="width: 260px" @keyup.enter="handleSearch">
              <template #append>
                <el-button @click="handleSearch"><el-icon><Search /></el-icon></el-button>
              </template>
            </el-input>
            <el-button v-if="canEdit" type="primary" @click="openDialog()">
              <el-icon><Plus /></el-icon> 添加资产
            </el-button>
          </div>
        </div>
      </template>

      <el-table :data="assetList" v-loading="loading" stripe border>
        <el-table-column prop="code" label="编号" width="120" />
        <el-table-column prop="name" label="名称" width="180" show-overflow-tooltip />
        <el-table-column prop="category" label="分类" width="120" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="statusType(scope.row.status)" size="small">
              {{ statusLabel(scope.row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ip" label="IP地址" width="140" />
        <el-table-column prop="purpose" label="用途" show-overflow-tooltip />
        <el-table-column prop="owner" label="责任人" width="120" />
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="scope">
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
          @size-change="fetchAssets"
          @current-change="fetchAssets"
        />
      </div>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑资产' : '添加资产'" width="600px" destroy-on-close>
      <el-form :model="form" :rules="formRules" ref="formRef" label-width="100px">
        <el-form-item label="资产编号" prop="code">
          <el-input v-model="form.code" placeholder="如：SRV-2024-001" />
        </el-form-item>
        <el-form-item label="资产名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入资产名称" />
        </el-form-item>
        <el-form-item label="分类" prop="category">
          <el-select v-model="form.category" placeholder="请选择分类" style="width: 100%">
            <el-option label="服务器" value="服务器" />
            <el-option label="网络设备" value="网络设备" />
            <el-option label="存储设备" value="存储设备" />
            <el-option label="安全设备" value="安全设备" />
            <el-option label="办公设备" value="办公设备" />
            <el-option label="其他" value="其他" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态" prop="status">
          <el-select v-model="form.status" placeholder="请选择状态" style="width: 100%">
            <el-option label="在用" value="active" />
            <el-option label="闲置" value="idle" />
            <el-option label="待报废" value="pending_scrap" />
            <el-option label="已报废" value="scrapped" />
          </el-select>
        </el-form-item>
        <el-form-item label="IP地址" prop="ip">
          <el-input v-model="form.ip" placeholder="请输入IP地址" />
        </el-form-item>
        <el-form-item label="用途" prop="purpose">
          <el-input v-model="form.purpose" type="textarea" rows="2" placeholder="请输入用途描述" />
        </el-form-item>
        <el-form-item label="责任人" prop="owner">
          <el-input v-model="form.owner" placeholder="请输入责任人" />
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

    <el-dialog v-model="detailVisible" title="资产详情" width="500px">
      <el-descriptions :column="1" border>
        <el-descriptions-item label="编号">{{ detailData.code }}</el-descriptions-item>
        <el-descriptions-item label="名称">{{ detailData.name }}</el-descriptions-item>
        <el-descriptions-item label="分类">{{ detailData.category }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <el-tag :type="statusType(detailData.status)" size="small">{{ statusLabel(detailData.status) }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="IP地址">{{ detailData.ip }}</el-descriptions-item>
        <el-descriptions-item label="用途">{{ detailData.purpose }}</el-descriptions-item>
        <el-descriptions-item label="责任人">{{ detailData.owner }}</el-descriptions-item>
        <el-descriptions-item label="备注">{{ detailData.remark }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">{{ detailData.created_at }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { assetApi } from '../api'

const loading = ref(false)
const searchQuery = ref('')
const assetList = ref([])
const pagination = reactive({
  page: 1,
  pageSize: 10,
  total: 0
})

const userStr = localStorage.getItem('user')
const user = userStr ? JSON.parse(userStr) : {}
const canEdit = computed(() => ['admin', 'assetmgr'].includes(user.role))

const statusMap = {
  active: { label: '在用', type: 'success' },
  idle: { label: '闲置', type: 'warning' },
  pending_scrap: { label: '待报废', type: 'danger' },
  scrapped: { label: '已报废', type: 'info' }
}

const statusType = (status) => statusMap[status]?.type || 'info'
const statusLabel = (status) => statusMap[status]?.label || status

const dialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref()
const form = reactive({
  id: null,
  code: '',
  name: '',
  category: '',
  status: 'active',
  ip: '',
  purpose: '',
  owner: '',
  remark: ''
})

const formRules = {
  code: [{ required: true, message: '请输入资产编号', trigger: 'blur' }],
  name: [{ required: true, message: '请输入资产名称', trigger: 'blur' }],
  category: [{ required: true, message: '请选择分类', trigger: 'change' }],
  status: [{ required: true, message: '请选择状态', trigger: 'change' }]
}

const detailVisible = ref(false)
const detailData = ref({})

const fetchAssets = async () => {
  loading.value = true
  try {
    const res = await assetApi.list({
      page: pagination.page,
      pageSize: pagination.pageSize,
      keyword: searchQuery.value
    })
    assetList.value = res.data?.list || []
    pagination.total = res.data?.total || 0
  } catch (e) {
    ElMessage.error('获取资产列表失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  pagination.page = 1
  fetchAssets()
}

const openDialog = (row) => {
  isEdit.value = !!row
  if (row) {
    Object.assign(form, row)
  } else {
    Object.keys(form).forEach(key => {
      form[key] = key === 'status' ? 'active' : ''
    })
    form.id = null
  }
  dialogVisible.value = true
}

const submitForm = async () => {
  try {
    await formRef.value.validate()
    if (isEdit.value) {
      await assetApi.update(form.id, form)
      ElMessage.success('更新成功')
    } else {
      await assetApi.create(form)
      ElMessage.success('添加成功')
    }
    dialogVisible.value = false
    fetchAssets()
  } catch (e) {
    if (e?.response) {
      ElMessage.error(e.response.data?.message || '操作失败')
    }
  }
}

const handleDelete = (row) => {
  ElMessageBox.confirm(`确定删除资产 "${row.name}" 吗？`, '提示', {
    type: 'warning'
  }).then(async () => {
    await assetApi.delete(row.id)
    ElMessage.success('删除成功')
    fetchAssets()
  }).catch(() => {})
}

const viewDetail = (row) => {
  detailData.value = row
  detailVisible.value = true
}

onMounted(fetchAssets)
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

.header-actions {
  display: flex;
  gap: 12px;
}

.pagination-wrapper {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}
</style>
