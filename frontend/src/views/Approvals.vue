<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span class="title">审批中心</span>
          <el-button type="primary" @click="openApplyDialog">
            <el-icon><Plus /></el-icon> 提交申请
          </el-button>
        </div>
      </template>

      <el-tabs v-model="activeTab">
        <el-tab-pane label="我的申请" name="my">
          <el-table :data="myApplications" v-loading="loading" stripe border>
            <el-table-column prop="type" label="类型" width="120" />
            <el-table-column prop="target_name" label="目标" width="180" show-overflow-tooltip />
            <el-table-column prop="reason" label="原因" show-overflow-tooltip />
            <el-table-column prop="status" label="状态" width="100">
              <template #default="scope">
                <el-tag :type="statusType(scope.row.status)" size="small">{{ statusLabel(scope.row.status) }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="approver_name" label="审批人" width="120" />
            <el-table-column prop="created_at" label="申请时间" width="160" />
            <el-table-column prop="result_remark" label="审批意见" show-overflow-tooltip />
          </el-table>
          <div class="pagination-wrapper">
            <el-pagination
              v-model:current-page="myPagination.page"
              v-model:page-size="myPagination.pageSize"
              :total="myPagination.total"
              layout="total, prev, pager, next"
              @current-change="fetchMyApplications"
            />
          </div>
        </el-tab-pane>

        <el-tab-pane label="待我审批" name="pending">
          <el-table :data="pendingApprovals" v-loading="loading" stripe border>
            <el-table-column prop="type" label="类型" width="120" />
            <el-table-column prop="applicant_name" label="申请人" width="120" />
            <el-table-column prop="target_name" label="目标" width="180" show-overflow-tooltip />
            <el-table-column prop="reason" label="原因" show-overflow-tooltip />
            <el-table-column prop="status" label="状态" width="100">
              <template #default="scope">
                <el-tag type="warning" size="small">待审批</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="created_at" label="申请时间" width="160" />
            <el-table-column label="操作" width="180" fixed="right">
              <template #default="scope">
                <el-button text type="success" size="small" @click="openApproveDialog(scope.row, 'approve')">通过</el-button>
                <el-button text type="danger" size="small" @click="openApproveDialog(scope.row, 'reject')">拒绝</el-button>
              </template>
            </el-table-column>
          </el-table>
          <div class="pagination-wrapper">
            <el-pagination
              v-model:current-page="pendingPagination.page"
              v-model:page-size="pendingPagination.pageSize"
              :total="pendingPagination.total"
              layout="total, prev, pager, next"
              @current-change="fetchPendingApprovals"
            />
          </div>
        </el-tab-pane>
      </el-tabs>
    </el-card>

    <el-dialog v-model="applyVisible" title="提交申请" width="500px" destroy-on-close>
      <el-form :model="applyForm" :rules="applyRules" ref="applyRef" label-width="100px">
        <el-form-item label="申请类型" prop="type">
          <el-select v-model="applyForm.type" placeholder="请选择类型" style="width: 100%">
            <el-option label="凭据访问" value="credential_access" />
            <el-option label="资产变更" value="asset_change" />
            <el-option label="权限申请" value="permission_request" />
          </el-select>
        </el-form-item>
        <el-form-item label="选择凭据" prop="target_id" v-if="applyForm.type === 'credential_access'">
          <el-select v-model="applyForm.target_id" placeholder="请选择凭据" filterable style="width: 100%">
            <el-option v-for="c in credentialOptions" :key="c.id" :label="c.name" :value="c.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="申请原因" prop="reason">
          <el-input v-model="applyForm.reason" type="textarea" rows="3" placeholder="请详细说明申请原因" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="applyVisible = false">取消</el-button>
        <el-button type="primary" @click="submitApply">提交</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="approveVisible" :title="approveAction === 'approve' ? '审批通过' : '审批拒绝'" width="500px">
      <p style="margin-bottom: 12px;">
        申请人：<strong>{{ currentApproveRow?.applicant_name }}</strong> &nbsp;|&nbsp;
        目标：<strong>{{ currentApproveRow?.target_name }}</strong>
      </p>
      <el-input v-model="approveRemark" type="textarea" rows="3" :placeholder="approveAction === 'approve' ? '请输入审批意见（可选）' : '请输入拒绝原因'" />
      <template #footer>
        <el-button @click="approveVisible = false">取消</el-button>
        <el-button :type="approveAction === 'approve' ? 'success' : 'danger'" @click="submitApprove">
          {{ approveAction === 'approve' ? '确认通过' : '确认拒绝' }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { approvalApi, credentialApi } from '../api'

const loading = ref(false)
const activeTab = ref('my')

const myApplications = ref([])
const myPagination = reactive({ page: 1, pageSize: 10, total: 0 })

const pendingApprovals = ref([])
const pendingPagination = reactive({ page: 1, pageSize: 10, total: 0 })

const statusMap = {
  pending: { label: '待审批', type: 'warning' },
  approved: { label: '已通过', type: 'success' },
  rejected: { label: '已拒绝', type: 'danger' }
}

const statusType = (status) => statusMap[status]?.type || 'info'
const statusLabel = (status) => statusMap[status]?.label || status

const applyVisible = ref(false)
const applyRef = ref()
const credentialOptions = ref([])
const applyForm = reactive({
  type: 'credential_access',
  target_id: '',
  reason: ''
})

const applyRules = {
  type: [{ required: true, message: '请选择类型', trigger: 'change' }],
  target_id: [{ required: true, message: '请选择目标', trigger: 'change' }],
  reason: [{ required: true, message: '请输入申请原因', trigger: 'blur' }]
}

const approveVisible = ref(false)
const approveAction = ref('approve')
const approveRemark = ref('')
const currentApproveRow = ref(null)

const fetchMyApplications = async () => {
  loading.value = true
  try {
    const res = await approvalApi.list({
      page: myPagination.page,
      pageSize: myPagination.pageSize,
      scope: 'my'
    })
    myApplications.value = res.data?.list || []
    myPagination.total = res.data?.total || 0
  } catch (e) {
    ElMessage.error('获取申请列表失败')
  } finally {
    loading.value = false
  }
}

const fetchPendingApprovals = async () => {
  loading.value = true
  try {
    const res = await approvalApi.list({
      page: pendingPagination.page,
      pageSize: pendingPagination.pageSize,
      scope: 'pending'
    })
    pendingApprovals.value = res.data?.list || []
    pendingPagination.total = res.data?.total || 0
  } catch (e) {
    ElMessage.error('获取待审批列表失败')
  } finally {
    loading.value = false
  }
}

const openApplyDialog = async () => {
  applyForm.type = 'credential_access'
  applyForm.target_id = ''
  applyForm.reason = ''
  try {
    const res = await credentialApi.list({ pageSize: 1000 })
    credentialOptions.value = res.data?.list || []
  } catch (e) {
    console.error(e)
  }
  applyVisible.value = true
}

const submitApply = async () => {
  try {
    await applyRef.value.validate()
    const target = credentialOptions.value.find(c => c.id === applyForm.target_id)
    await approvalApi.create({
      type: applyForm.type,
      target_id: applyForm.target_id,
      target_name: target?.name || '',
      reason: applyForm.reason
    })
    ElMessage.success('申请已提交')
    applyVisible.value = false
    fetchMyApplications()
  } catch (e) {
    if (e?.response) {
      ElMessage.error(e.response.data?.message || '提交失败')
    }
  }
}

const openApproveDialog = (row, action) => {
  currentApproveRow.value = row
  approveAction.value = action
  approveRemark.value = ''
  approveVisible.value = true
}

const submitApprove = async () => {
  if (approveAction.value === 'reject' && !approveRemark.value.trim()) {
    ElMessage.warning('请填写拒绝原因')
    return
  }
  try {
    await approvalApi.update(currentApproveRow.value.id, {
      action: approveAction.value,
      remark: approveRemark.value
    })
    ElMessage.success(approveAction.value === 'approve' ? '审批通过' : '已拒绝')
    approveVisible.value = false
    fetchPendingApprovals()
    fetchMyApplications()
  } catch (e) {
    ElMessage.error(e.response?.data?.message || '操作失败')
  }
}

onMounted(() => {
  fetchMyApplications()
  fetchPendingApprovals()
})
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

.pagination-wrapper {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}
</style>
