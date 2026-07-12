import axios from 'axios'

const instance = axios.create({
  baseURL: '/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

instance.interceptors.request.use(
  (config) => {
    const userStr = localStorage.getItem('user')
    if (userStr) {
      const user = JSON.parse(userStr)
      config.headers.Authorization = `Bearer ${user.token || ''}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

instance.interceptors.response.use(
  (response) => {
    const data = response.data
    // 统一包装：如果后端直接返回数组，包装为标准分页格式
    if (Array.isArray(data)) {
      return { data: { list: data, total: data.length } }
    }
    return data
  },
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

const createApi = (resource) => ({
  list: (params) => instance.get(`/${resource}`, { params }),
  get: (id) => instance.get(`/${resource}/${id}`),
  create: (data) => instance.post(`/${resource}`, data),
  update: (id, data) => instance.put(`/${resource}/${id}`, data),
  delete: (id) => instance.delete(`/${resource}/${id}`)
})

export const authApi = {
  login: (data) => instance.post('/auth/login', data),
  logout: () => instance.post('/auth/logout'),
  me: () => instance.get('/auth/me')
}

export const assetApi = createApi('assets')
const credentialBase = createApi('credentials')
export const credentialApi = {
  ...credentialBase,
  reveal: (id, data) => instance.post(`/credentials/${id}/reveal`, data)
}
export const approvalApi = createApi('approvals')
export const auditApi = createApi('audit')
export const userApi = createApi('users')

export default instance
