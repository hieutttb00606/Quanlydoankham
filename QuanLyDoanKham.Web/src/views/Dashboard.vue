<template>
  <div class="min-h-screen flex flex-col bg-slate-50 font-sans">
    <!-- Updated Navigation Bar -->
    <nav class="bg-white border-b border-slate-100 sticky top-0 z-50 shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-20 items-center">
          <div class="flex items-center space-x-12">
            <div class="flex items-center cursor-pointer group" @click="activeMenu = 'home'">
              <div class="bg-primary p-2.5 rounded-xl group-hover:rotate-12 transition-transform duration-300 shadow-lg shadow-primary/20">
                <Stethoscope class="text-white w-7 h-7" />
              </div>
              <span class="ml-4 font-black text-3xl text-primary tracking-tight">Health<span class="text-slate-800">Care</span></span>
            </div>
            
            <div class="hidden lg:flex space-x-8">
              <span v-for="item in filteredMenuItems.slice(1)" :key="item.id" 
                    @click="activeMenu = item.id"
                    :class="['nav-link text-sm uppercase tracking-widest font-black', activeMenu === item.id ? 'text-primary border-b-4 border-primary pb-1' : 'text-slate-400']">
                {{ item.name }}
              </span>
            </div>
          </div>

          <div class="flex items-center space-x-6">
            <div class="flex items-center space-x-2 text-primary font-black bg-primary-light px-4 py-2 rounded-full cursor-pointer hover:bg-primary hover:text-white transition-all">
              <Sparkles class="w-4 h-4" />
              <span class="text-xs uppercase">AI Hợp tác</span>
            </div>
            <div class="h-8 w-px bg-slate-100"></div>
            <div class="flex items-center space-x-4">
              <div class="text-right">
                <p class="text-sm font-black text-slate-800 leading-none capitalize mb-1">{{ authStore.user?.username }}</p>
                <p class="text-[10px] text-primary font-black uppercase tracking-widest bg-blue-50 px-2 py-0.5 rounded">{{ authStore.user?.role }}</p>
              </div>
              <button @click="handleLogout" class="bg-slate-50 p-2.5 rounded-xl hover:bg-rose-50 hover:text-rose-600 transition-all text-slate-400 group">
                <LogOut class="w-5 h-5 group-hover:scale-110 transition-transform" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main Content Area -->
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 flex-1 w-full">
      <!-- Logic: Show Home Dashboard OR specific Module -->
      <div v-if="activeMenu === 'home'" class="animate-fade-in">
          <!-- Hero Section -->
          <div class="text-center mb-16">
            <h1 class="text-5xl md:text-6xl font-black text-slate-900 mb-6 tracking-tighter leading-tight">
                Chào mừng ngày mới, <br/> 
                <span class="text-primary tracking-normal italic">{{ authStore.user?.username }}!</span>
            </h1>
            <p class="text-slate-500 font-bold text-lg max-w-2xl mx-auto mb-10">
                Theo dõi và quản lý các hoạt động của đoàn khám ngay tại trung tâm điều khiển này.
            </p>
            
            <!-- Quick Search -->
            <div class="max-w-3xl mx-auto relative shadow-2xl rounded-[2.5rem] p-1 bg-white ring-8 ring-slate-100">
                <div class="absolute inset-y-0 left-0 pl-8 flex items-center pointer-events-none text-slate-400">
                <Search class="w-6 h-6" />
                </div>
                <input type="text" 
                v-model="searchQuery"
                @input="handleSearch"
                placeholder="Tìm kiếm nhanh công ty hoặc hợp đồng..."
                class="block w-full pl-16 pr-8 py-6 bg-transparent border-none rounded-[2rem] text-xl font-bold placeholder-slate-300 focus:outline-none">
                
                <!-- Search Results Dropdown -->
                <div v-if="searchResults.length > 0 && searchQuery" class="absolute top-full left-0 right-0 mt-4 bg-white rounded-[2rem] shadow-2xl border border-slate-100 overflow-hidden z-[100] animate-fade-in-up">
                    <div v-for="res in searchResults" :key="res.id" 
                         @click="navigateSearchResult(res)"
                         class="flex items-center p-6 hover:bg-slate-50 cursor-pointer border-b border-slate-50 last:border-none transition-all group">
                        <div :class="['w-12 h-12 rounded-xl flex items-center justify-center mr-6 group-hover:scale-110 transition-transform', 
                             res.type === 'company' ? 'bg-sky-50 text-sky-600' : 
                             res.type === 'contract' ? 'bg-teal-50 text-teal-600' :
                             res.type === 'staff' ? 'bg-rose-50 text-rose-600' :
                             res.type === 'group' ? 'bg-amber-50 text-amber-600' :
                             'bg-violet-50 text-violet-600']">
                            <component :is="getSearchIcon(res.type)" class="w-6 h-6" />
                        </div>
                        <div class="text-left">
                            <p class="font-black text-slate-800">{{ res.name }}</p>
                            <p class="text-xs font-black uppercase tracking-widest text-slate-400">
                                {{ res.type === 'company' ? 'Công ty • Đối tác' : 
                                   res.type === 'contract' ? 'Hợp đồng • #'+res.id :
                                   res.type === 'staff' ? 'Nhân sự • ' + res.sub :
                                   res.type === 'group' ? 'Đoàn khám • ' + res.sub :
                                   'Vật tư • Kho' }}
                            </p>
                        </div>
                        <ArrowRight class="ml-auto w-5 h-5 text-slate-300 group-hover:text-primary transition-colors" />
                    </div>
                </div>

                <div v-if="searchQuery && searchResults.length === 0" class="absolute top-full left-0 right-0 mt-4 bg-white rounded-[2rem] shadow-2xl p-8 text-slate-400 font-bold animate-fade-in">
                    Không tìm thấy kết quả phù hợp...
                </div>

                <button class="absolute right-3 inset-y-3 px-10 bg-primary hover:bg-primary-dark text-white rounded-[1.5rem] font-black tracking-widest text-xs uppercase shadow-xl transition-all active:scale-95">
                TÌM KIẾM
                </button>
            </div>
          </div>

          <!-- Feature Cards for Navigation -->
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            <div v-for="item in activeModules" :key="item.id" 
                @click="activeMenu = item.id"
                class="group bg-white p-10 rounded-[3rem] border border-slate-100 shadow-sm hover:border-primary/20 hover:shadow-2xl transition-all cursor-pointer flex flex-col items-center text-center">
                <div :class="['w-24 h-24 rounded-[2rem] flex items-center justify-center mb-8 shadow-inner transition-transform group-hover:scale-110 duration-500', item.color]">
                    <component :is="item.icon" class="w-10 h-10" />
                </div>
                <h3 class="text-2xl font-black text-slate-900 mb-3 tracking-tight group-hover:text-primary transition-colors">{{ item.name }}</h3>
                <p class="text-slate-400 font-medium text-sm leading-relaxed mb-8">{{ item.desc }}</p>
                <div class="flex items-center space-x-2 text-xs font-black uppercase tracking-widest text-primary opacity-0 group-hover:opacity-100 transition-all">
                    <span>Mở Module</span>
                    <ArrowRight class="w-4 h-4" />
                </div>
            </div>
          </div>
      </div>

      <!-- Specific Components Implementation -->
      <div v-else class="animate-slide-up">
          <Companies v-if="activeMenu === 'companies'" />
          <Contracts v-if="activeMenu === 'contracts'" />
          <Staff v-if="activeMenu === 'staff'" />
          <Groups v-if="activeMenu === 'groups'" />
          <Supplies v-if="activeMenu === 'supplies'" />
          <Reports v-if="activeMenu === 'reports'" />
          <AccountManager v-if="activeMenu === 'users'" />
          
          <!-- Placeholder for other modules -->
          <div v-if="!['companies', 'contracts', 'staff', 'groups', 'supplies', 'reports', 'users'].includes(activeMenu)" class="flex flex-col items-center justify-center py-32 bg-white rounded-[4rem] border-4 border-dashed border-slate-100">
             <div class="bg-slate-50 p-10 rounded-full mb-8">
                <component :is="activeIcon" class="w-20 h-20 text-slate-200" />
             </div>
             <p class="text-2xl font-black text-slate-300">Module {{ activeMenuName }} đang được hoàn thiện</p>
             <button @click="activeMenu = 'home'" class="mt-8 text-primary font-black hover:underline flex items-center space-x-2">
                <ArrowLeft class="w-4 h-4" /> <span>Về trang chủ</span>
             </button>
          </div>
      </div>
    </main>

    <!-- Professional Footer -->
    <footer class="bg-white border-t border-slate-100 py-12 mt-20">
      <div class="max-w-7xl mx-auto px-8 flex flex-col md:flex-row justify-between items-center text-slate-400 text-sm font-bold">
        <div class="flex items-center space-x-4 mb-4 md:mb-0">
             <Stethoscope class="w-6 h-6 text-primary" />
             <span class="text-slate-800">HealthCare Management © 2026</span>
        </div>
        <div class="flex space-x-12">
            <span class="hover:text-primary cursor-pointer">Bảo mật</span>
            <span class="hover:text-primary cursor-pointer">Điều khoản</span>
            <span class="hover:text-primary cursor-pointer">Hỗ trợ 24/7</span>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { 
  Stethoscope, Building2, FileText, Users as UsersIcon, Package, BarChart3, 
  LogOut, Search, ArrowRight, ArrowLeft, Sparkles, Bot, Shield
} from 'lucide-vue-next'

// Import Modules
import Companies from './Companies.vue'
import Contracts from './Contracts.vue'
import Staff from './Staff.vue'
import Groups from './Groups.vue'
import Supplies from './Supplies.vue'
import Reports from './Reports.vue'
import AccountManager from './Users.vue'

const router = useRouter()
const authStore = useAuthStore()
const activeMenu = ref('home')

const menuItems = [
  { id: 'home', name: 'Trang chủ', icon: Bot, color: 'bg-indigo-50 text-indigo-600', desc: 'Trung tâm điều khiển và phân tích dữ liệu.' },
  { id: 'companies', name: 'Công ty', icon: Building2, color: 'bg-sky-50 text-sky-600', desc: 'Quản lý thông tin đối tác và doanh nghiệp.', roles: ['Admin', 'Staff'] },
  { id: 'contracts', name: 'Hợp đồng', icon: FileText, color: 'bg-teal-50 text-teal-600', desc: 'Hệ thống lưu trữ và quản lý hợp đồng y tế.', roles: ['Admin', 'Staff', 'Customer'] },
  { id: 'staff', name: 'Nhân sự', icon: UsersIcon, color: 'bg-rose-50 text-rose-600', desc: 'Phân bổ đội ngũ y bác sĩ cho các đoàn.', roles: ['Admin', 'Staff'] },
  { id: 'groups', name: 'Đoàn khám', icon: Stethoscope, color: 'bg-amber-50 text-amber-600', desc: 'Vận hành và giám sát quy trình khám.', roles: ['Admin', 'Staff', 'Customer'] },
  { id: 'supplies', name: 'Vật tư', icon: Package, color: 'bg-violet-50 text-violet-600', desc: 'Quản lý kho thiết bị và tiêu hao.', roles: ['Admin', 'Staff'] },
  { id: 'reports', name: 'Báo cáo', icon: BarChart3, color: 'bg-emerald-50 text-emerald-600', desc: 'Biểu đồ phân tích doanh thu thời gian thực.', roles: ['Admin'] },
  { id: 'users', name: 'Tài khoản', icon: Shield, color: 'bg-slate-50 text-slate-600', desc: 'Quản lý phân quyền và người dùng.', roles: ['Admin'] },
]

const activeModules = computed(() => {
    return menuItems.filter(i => {
        if (i.id === 'home') return false;
        if (!i.roles) return true;
        return i.roles.includes(authStore.role);
    });
})

const filteredMenuItems = computed(() => {
    return menuItems.filter(i => {
        if (!i.roles) return true;
        return i.roles.includes(authStore.role);
    });
})

const activeMenuName = computed(() => menuItems.find(i => i.id === activeMenu.value)?.name || '')
const activeIcon = computed(() => menuItems.find(i => i.id === activeMenu.value)?.icon || Bot)

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}

// Search Logic
import axios from 'axios'
import { onMounted, watch } from 'vue'
const searchQuery = ref('')
const searchResults = ref([])
const isDataSyncing = ref(false)

const navigateSearchResult = (res) => {
    searchQuery.value = ''
    searchResults.value = []
    activeMenu.value = res.target
}

const getSearchIcon = (type) => {
    switch(type) {
        case 'company':
        case 'companies': return Building2
        case 'contract':
        case 'contracts': return FileText
        case 'staff': return UsersIcon
        case 'group':
        case 'groups': return Stethoscope
        case 'supply':
        case 'supplies': return Package
        case 'reports': return BarChart3
        case 'users': return Shield
        default: return Bot
    }
}

// Full System Search Logic - Fetch on Load for faster search
const allData = ref({ companies: [], contracts: [], staff: [], groups: [], supplies: [] })

const fetchSearchData = async () => {
    if (isDataSyncing.value) return
    isDataSyncing.value = true
    try {
        // Individual fetching to prevent one failure from blocking all
        const endpoints = [
            { key: 'companies', url: 'http://localhost:5283/api/Companies' },
            { key: 'contracts', url: 'http://localhost:5283/api/HealthContracts' },
            { key: 'staff', url: 'http://localhost:5283/api/Staffs' },
            { key: 'groups', url: 'http://localhost:5283/api/MedicalGroups' },
            { key: 'supplies', url: 'http://localhost:5283/api/Supplies' }
        ]

        await Promise.all(endpoints.map(async (ep) => {
            try {
                const res = await axios.get(ep.url)
                allData.value[ep.key] = res.data
            } catch (err) {
                console.warn(`Could not sync ${ep.key} for search`, err)
            }
        }))
    } catch (e) {
        console.error("Global search sync failed", e)
    } finally {
        isDataSyncing.value = false
    }
}

const handleSearch = () => {
    if (!searchQuery.value) {
        searchResults.value = []
        return
    }

    const q = searchQuery.value.toLowerCase().trim()
    const matches = []

    // 0. Module Quick Access (Logic for "Quick Jump")
    const modules = [
        { key: 'companies', names: ['công ty', 'doanh nghiệp', 'đối tác'], label: 'Quản lý Công ty/Đối tác', icon: Building2, roles: ['Admin', 'Staff'] },
        { key: 'contracts', names: ['hợp đồng', 'hd', 'hop dong'], label: 'Hệ thống Hợp đồng y tế', icon: FileText, roles: ['Admin', 'Staff', 'Customer'] },
        { key: 'staff', names: ['nhân sự', 'nhân viên', 'bác sĩ', 'điều dưỡng', 'nhan su'], label: 'Quản lý Đội ngũ Nhân sự', icon: UsersIcon, roles: ['Admin', 'Staff'] },
        { key: 'groups', names: ['đoàn khám', 'kế hoạch', 'doan kham'], label: 'Vận hành Đoàn khám', icon: Stethoscope, roles: ['Admin', 'Staff', 'Customer'] },
        { key: 'supplies', names: ['vật tư', 'kho', 'thiết bị', 'vat tu'], label: 'Quản lý Kho & Vật tư', icon: Package, roles: ['Admin', 'Staff'] },
        { key: 'reports', names: ['báo cáo', 'doanh thu', 'thống kê', 'bao cao'], label: 'Báo cáo & Thống kê doanh thu', icon: BarChart3, roles: ['Admin'] },
        { key: 'users', names: ['tài khoản', 'người dùng', 'phân quyền', 'tai khoan'], label: 'Quản lý Tài khoản & Phân quyền', icon: Shield, roles: ['Admin'] }
    ]

    modules.forEach(m => {
        const hasRole = !m.roles || m.roles.includes(authStore.role)
        if (hasRole && (m.names.some(n => n.includes(q)) || q.includes(m.key))) {
            matches.push({ 
                id: `module-${m.key}`, 
                name: m.label, 
                type: m.key, 
                target: m.key, 
                isQuickLink: true 
            })
        }
    })

    // Helper: Match strict or smart
    const matchesQuery = (text) => text?.toLowerCase().includes(q)

    // 1. Search Companies
    if (Array.isArray(allData.value.companies)) {
        allData.value.companies.forEach(c => {
            if (matchesQuery(c.companyName)) {
                matches.push({ id: c.companyId, name: c.companyName, type: 'company', target: 'companies' })
            }
        })
    }

    // 2. Search Contracts
    if (Array.isArray(allData.value.contracts)) {
        allData.value.contracts.forEach(c => {
            const displayTitle = `Hợp đồng ${c.companyName || 'N/A'}`
            const idMatch = c.healthContractId?.toString().includes(q)
            if (matchesQuery(c.companyName) || idMatch) {
                matches.push({ id: c.healthContractId, name: displayTitle, type: 'contract', target: 'contracts' })
            }
        })
    }

    // 3. Search Staff
    if (Array.isArray(allData.value.staff)) {
        allData.value.staff.forEach(s => {
            // Only show staff if specific name/job matches (not just "staff" keyword)
            if (matchesQuery(s.fullName) || (q.length > 3 && matchesQuery(s.jobTitle))) {
                matches.push({ id: s.staffId, name: s.fullName, type: 'staff', target: 'staff', sub: s.jobTitle })
            }
        })
    }

    // 4. Search Groups
    if (Array.isArray(allData.value.groups)) {
        allData.value.groups.forEach(g => {
            if (matchesQuery(g.groupName)) {
                matches.push({ id: g.groupId, name: g.groupName, type: 'group', target: 'groups', sub: g.examDate ? new Date(g.examDate).toLocaleDateString('vi-VN') : '' })
            }
        })
    }

    // 5. Search Supplies
    if (Array.isArray(allData.value.supplies)) {
        allData.value.supplies.forEach(s => {
            if (matchesQuery(s.supplyName)) {
                matches.push({ id: s.supplyId, name: s.supplyName, type: 'supply', target: 'supplies', sub: s.isFixedAsset ? 'Cố định' : 'Tiêu hao' })
            }
        })
    }

    // Sort: Quick links first, then relevance
    matches.sort((a, b) => {
        if (a.isQuickLink && !b.isQuickLink) return -1
        if (!a.isQuickLink && b.isQuickLink) return 1
        return a.name.toLowerCase().startsWith(q) ? -1 : 1
    })

    searchResults.value = matches.slice(0, 10) 
}

onMounted(() => {
    fetchSearchData()
})

watch(searchQuery, (newVal) => {
    if (newVal === '') searchResults.value = []
    if (newVal.length === 1) fetchSearchData()
})
</script>

<style scoped>
/* Dashboard Specific Styles */
</style>
