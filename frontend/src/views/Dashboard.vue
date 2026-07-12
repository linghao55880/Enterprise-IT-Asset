<template>
  <div class="dashboard-page">
    <el-row :gutter="20">
      <el-col :xs="24" :sm="12" :lg="6">
        <el-card shadow="hover">
          <el-statistic title="资产总数" :value="stats.assetCount">
            <template #prefix>
              <el-icon :size="20" color="#409EFF"><Box /></el-icon>
            </template>
          </el-statistic>
        </el-card>
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <el-card shadow="hover">
          <el-statistic title="凭据总数" :value="stats.credentialCount">
            <template #prefix>
              <el-icon :size="20" color="#67C23A"><Key /></el-icon>
            </template>
          </el-statistic>
        </el-card>
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <el-card shadow="hover">
          <el-statistic title="待审批数" :value="stats.pendingApprovalCount">
            <template #prefix>
              <el-icon :size="20" color="#E6A23C"><DocumentChecked /></el-icon>
            </template>
          </el-statistic>
        </el-card>
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <el-card shadow="hover">
          <el-statistic title="今日操作数" :value="stats.todayActionCount">
            <template #prefix>
              <el-icon :size="20" color="#F56C6C"><Timer /></el-icon>
            </template>
          </el-statistic>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="20" class="mt-20">
      <el-col :xs="24" :lg="12">
        <el-card shadow="hover" title="资产状态分布">
          <template #header>
            <div class="card-header">
              <span>资产状态分布</span>
            </div>
          </template>
          <div class="status-list">
            <div class="status-item" v-for="item in statusDistribution" :key="item.status">
              <div class="status-label">
                <el-tag :type="item.type" size="small">{{ item.label }}</el-tag>
                <span class="status-count">{{ item.count }}</span>
              </div>
              <el-progress :percentage="item.percentage" :color="item.color" :stroke-width="12" />
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="24" :lg="12">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>最近操作</span>
              <el-button text type="primary" size="small" @click="$router.push('/audit')">查看更多</el-button>
            </div>
          </template>
          <el-table :data="recentLogs" size="small" stripe>
            <el-table-column prop="created_at" label="时间" width="160" />
            <el-table-column prop="username" label="用户" width="100" />
            <el-table-column prop="action" label="操作" width="100">
              <template #default="scope">
                <el-tag size="small" :type="scope.row.result === 'success' ? 'success' : 'danger'">{{ scope.row.action }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="object" label="对象" show-overflow-tooltip />
          </el-table>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { assetApi, credentialApi, approvalApi, auditApi } from '../api'

const stats = ref({
  assetCount: 0,
  credentialCount: 0,
  pendingApprovalCount: 0,
  todayActionCount: 0
})

const statusDistribution = ref([])
const recentLogs = ref([])

const fetchStats = async () => {
  try {
    const [assets, credentials, approvals, audits] = await Promise.all([
      assetApi.list({ pageSize: 1 }),
      credentialApi.list({ pageSize: 1 }),
      approvalApi.list({ status: 'pending', pageSize: 1 }),
      auditApi.list({ pageSize: 5 })
    ])

    stats.value.assetCount = assets.data?.total || 0
    stats.value.credentialCount = credentials.data?.total || 0
    stats.value.pendingApprovalCount = approvals.data?.total || 0
    stats.value.todayActionCount = audits.data?.total || 0

    const assetList = await assetApi.list({ pageSize: 1000 })
    const list = assetList.data?.list || []
    const total = list.length || 1
    const statusMap = {
      active: { label: '在用', type: 'success', color: '#67C23A' },
      idle: { label: '闲置', type: 'warning', color: '#E6A23C' },
      pending_scrap: { label: '待报废', type: 'danger', color: '#F56C6C' },
      scrapped: { label: '已报废', type: 'info', color: '#909399' }
    }

    const counts = {}
    list.forEach(item => {
      counts[item.status] = (counts[item.status] || 0) + 1
    })

    statusDistribution.value = Object.keys(statusMap).map(key => ({
      status: key,
      label: statusMap[key].label,
      type: statusMap[key].type,
      color: statusMap[key].color,
      count: counts[key] || 0,
      percentage: Math.round(((counts[key] || 0) / total) * 100)
    }))

    recentLogs.value = (audits.data?.list || []).slice(0, 5)
  } catch (e) {
    console.error(e)
  }
}

onMounted(fetchStats)
</script>

<style scoped>
.mt-20 {
  margin-top: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: 600;
}

.status-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.status-item {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.status-label {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.status-count {
  font-weight: bold;
  color: #303133;
}
</style>
