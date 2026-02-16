<template>
  <div class="space-y-6 animate-fade-in pb-20">
    <div class="flex justify-between items-center text-slate-800">
      <h2 class="text-3xl font-black">{{ isAdmin ? 'Hệ thống Tài khoản' : 'Thông tin Cá nhân' }}</h2>
      <button v-if="isAdmin" @click="openCreateModal" 
              class="bg-indigo-600 hover:bg-indigo-700 text-white px-6 py-3 rounded-2xl font-bold flex items-center space-x-2 transition-all shadow-lg active:scale-95">
        <UserPlus class="w-5 h-5" />
        <span>Cấp tài khoản mới</span>
      </button>
    </div>

    <!-- ADMIN VIEW: USER LIST -->
    <div v-if="isAdmin" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
        <div v-for="u in users" :key="u.username" class="bg-white p-8 rounded-[2.5rem] border border-slate-100 shadow-xl hover:shadow-2xl transition-all group relative overflow-hidden">
            <!-- Background Decoration for Roles -->
            <div :class="['absolute -right-6 -top-6 w-32 h-32 rounded-full opacity-5 blur-xl', 
                u.roleName === 'Admin' ? 'bg-indigo-500' : 
                u.roleName === 'Staff' ? 'bg-emerald-500' : 'bg-amber-500']"></div>

            <div class="flex items-center space-x-5 relative z-10">
                <div class="w-20 h-20 rounded-[2rem] bg-slate-50 border-2 border-indigo-50 p-1 shadow-inner overflow-hidden flex-shrink-0">
                    <img :src="`https://api.dicebear.com/7.x/avataaars/svg?seed=${u.username}`" class="w-full h-full object-cover" />
                </div>
                <div>
                    <h4 class="text-xl font-black text-slate-800">{{ u.fullname || 'Chưa đặt tên' }}</h4>
                    <p class="text-sm font-bold text-slate-400">@{{ u.username }}</p>
                </div>
            </div>

            <div class="mt-8 space-y-4">
                <div class="flex justify-between items-center text-xs">
                    <span class="font-black text-slate-400 uppercase tracking-widest">Vai trò</span>
                    <span :class="['px-4 py-1.5 rounded-full font-black uppercase text-[10px] tracking-widest shadow-sm',
                        u.roleName === 'Admin' ? 'bg-indigo-600 text-white' : 
                        u.roleName === 'Staff' ? 'bg-emerald-500 text-white' : 'bg-amber-400 text-white']">
                        {{ u.roleName }}
                    </span>
                </div>
                <div class="flex justify-between items-center text-xs">
                    <span class="font-black text-slate-400 uppercase tracking-widest">Đơn vị</span>
                    <span class="font-black text-slate-700 truncate max-w-[150px]">{{ u.companyName || 'HealthCare HQ' }}</span>
                </div>
            </div>

            <div class="mt-8 pt-6 border-t border-slate-50 flex items-center justify-between space-x-3">
                <button @click="openEditModal(u)" class="flex-1 py-3 bg-slate-50 hover:bg-indigo-50 text-slate-600 hover:text-indigo-600 rounded-xl font-black text-xs transition-all flex items-center justify-center space-x-2">
                    <Edit3 class="w-4 h-4" />
                    <span>CHỈNH SỬA</span>
                </button>
                <button v-if="u.username !== 'admin'" @click="handleDelete(u.username)" class="w-12 h-12 bg-rose-50 hover:bg-rose-500 text-rose-500 hover:text-white rounded-xl transition-all flex items-center justify-center group/del">
                    <Trash2 class="w-5 h-5" />
                </button>
            </div>
            
            <div v-if="u.roleName === 'Admin'" class="absolute top-4 right-4 group-hover:animate-bounce">
                <Shield class="w-6 h-6 text-indigo-200" />
            </div>
        </div>
    </div>

    <!-- USER VIEW: PROFILE CARD -->
    <div v-else-if="profile" class="max-w-2xl mx-auto">
        <div class="bg-white rounded-[3rem] shadow-2xl overflow-hidden relative border border-slate-100">
            <!-- Header Cover -->
            <div class="h-48 bg-gradient-to-r from-indigo-500 via-purple-500 to-indigo-600 relative">
                <div class="absolute inset-0 opacity-10" style="background-image: radial-gradient(circle at 2px 2px, white 1px, transparent 0); background-size: 24px 24px;"></div>
            </div>

            <!-- Profile Info -->
            <div class="px-12 pb-12 relative">
                <div class="absolute -top-20 left-12">
                    <div class="w-40 h-40 rounded-[2.5rem] bg-white p-2 shadow-2xl border-4 border-white">
                        <img :src="`https://api.dicebear.com/7.x/avataaars/svg?seed=${profile.username}`" class="w-full h-full object-cover rounded-[2rem] bg-slate-50" />
                    </div>
                </div>

                <div class="pt-24 flex justify-between items-start">
                    <div>
                        <h3 class="text-4xl font-black text-slate-800">{{ profile.fullName }}</h3>
                        <p class="text-lg font-bold text-slate-400 mt-1">@{{ profile.username }}</p>
                    </div>
                    <div class="flex flex-col items-end space-y-2">
                        <span :class="['px-6 py-2 rounded-full text-sm font-black uppercase tracking-widest shadow-xl', 
                            profile.roleName === 'Staff' ? 'bg-emerald-500 text-white' : 'bg-amber-400 text-white']">
                            {{ profile.roleName }}
                        </span>
                        <div class="flex items-center space-x-2 text-slate-400 font-bold text-xs uppercase tracking-tighter">
                            <MapPin class="w-3 h-3" />
                            <span>{{ profile.companyName || 'HealthCare HQ' }}</span>
                        </div>
                    </div>
                </div>

                <div class="mt-12 grid grid-cols-1 md:grid-cols-2 gap-8">
                    <div class="bg-slate-50 p-8 rounded-[2.5rem] border border-slate-100 flex items-center space-x-5">
                        <div class="bg-indigo-100 p-4 rounded-2xl text-indigo-600">
                            <Mail class="w-6 h-6" />
                        </div>
                        <div>
                            <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest">Email Công vụ</p>
                            <p class="font-black text-slate-700">{{ profile.username }}@healthcare.vn</p>
                        </div>
                    </div>
                    <div class="bg-slate-50 p-8 rounded-[2.5rem] border border-slate-100 flex items-center space-x-5">
                        <div class="bg-indigo-100 p-4 rounded-2xl text-indigo-600">
                            <Calendar class="w-6 h-6" />
                        </div>
                        <div>
                            <p class="text-[10px] font-black text-slate-400 uppercase tracking-widest">Ngày tham gia</p>
                            <p class="font-black text-slate-700">Tháng 02, 2026</p>
                        </div>
                    </div>
                </div>

                <div class="mt-10 pt-10 border-t border-slate-100">
                    <h5 class="text-xs font-black text-slate-400 uppercase tracking-widest mb-6 px-2">Hoạt động gần đây</h5>
                    <div class="space-y-4">
                        <div v-for="i in 2" :key="i" class="flex items-center space-x-4 p-4 rounded-2xl bg-white border border-slate-50 hover:bg-slate-50 transition-all cursor-default group">
                            <div class="w-2 h-2 rounded-full bg-indigo-400 group-hover:scale-150 transition-transform"></div>
                            <p class="text-sm font-bold text-slate-600">Truy cập hệ thống vào {{ i === 1 ? 'Hôm nay' : 'Hôm qua' }} lúc 09:15 AM</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- MODAL: CREATE / EDIT USER -->
    <div v-if="modal.show" class="fixed inset-0 z-[100] flex items-center justify-center bg-slate-900/80 backdrop-blur-md p-4 overflow-y-auto">
        <div class="bg-white w-full max-w-xl p-10 rounded-[3rem] shadow-2xl relative animate-fade-in-up mt-auto mb-auto">
            <button @click="modal.show = false" class="absolute top-8 right-8 text-slate-300 hover:text-slate-800 transition-all">
                <X class="w-8 h-8" />
            </button>
            
            <h3 class="text-2xl font-black text-slate-800 flex items-center mb-8">
                <div class="w-10 h-10 bg-indigo-600 text-white rounded-2xl flex items-center justify-center mr-4 shadow-lg shadow-indigo-200">
                    <UserPlus v-if="!modal.isEdit" class="w-6 h-6" />
                    <Edit3 v-else class="w-6 h-6" />
                </div>
                {{ modal.isEdit ? 'Cập nhật tài khoản' : 'Cấp tài khoản mới' }}
            </h3>

            <form @submit.prevent="handleSubmit" class="space-y-6">
                <div class="space-y-2">
                    <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Tên đăng nhập</label>
                    <input v-model="form.username" :disabled="modal.isEdit" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-indigo-400/20 focus:bg-white outline-none transition-all font-bold disabled:opacity-50" />
                </div>
                
                <div class="space-y-2">
                    <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Họ và Tên</label>
                    <input v-model="form.fullName" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-indigo-400/20 focus:bg-white outline-none transition-all font-bold" />
                </div>

                <div class="grid grid-cols-2 gap-4">
                    <div class="space-y-2">
                        <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Vai trò</label>
                        <select v-model="form.roleId" class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-indigo-400/20 focus:bg-white outline-none transition-all font-bold">
                            <option :value="1">Admin</option>
                            <option :value="2">Staff</option>
                            <option :value="3">Customer</option>
                        </select>
                    </div>
                </div>

                <div v-show="form.roleId === 3" class="space-y-2">
                    <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Đơn vị / Công ty</label>
                    <select v-model="form.companyId" class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-indigo-400/20 focus:bg-white outline-none transition-all font-bold">
                        <option :value="null">-- Hệ thống --</option>
                        <option v-for="c in companies" :key="c.companyId" :value="c.companyId">{{ c.companyName }}</option>
                    </select>
                </div>

                <div class="space-y-2">
                    <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">
                        {{ modal.isEdit ? 'Đổi mật khẩu (Bỏ trống nếu giữ nguyên)' : 'Mật khẩu' }}
                    </label>
                    <input v-model="form.password" :required="!modal.isEdit" type="password" class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-indigo-400/20 focus:bg-white outline-none transition-all font-bold" />
                </div>

                <button type="submit" class="w-full bg-indigo-600 text-white py-5 rounded-2xl font-black shadow-xl shadow-indigo-100 hover:bg-indigo-700 transition-all tracking-widest uppercase">
                    {{ modal.isEdit ? 'LƯU THAY ĐỔI' : 'XÁC NHẬN TẠO MỚI' }}
                </button>
            </form>
        </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { useAuthStore } from '../stores/auth'
import { 
    UserPlus, Edit3, Trash2, Shield, X, MapPin, Mail, Calendar 
} from 'lucide-vue-next'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.role === 'Admin')
const profile = computed(() => authStore.profile)

const users = ref([])
const companies = ref([])
const modal = ref({ show: false, isEdit: false })
const form = ref({
    username: '',
    password: '',
    fullName: '',
    roleId: 2,
    companyId: null
})

const fetchUsers = async () => {
    if (!isAdmin.value) return
    try {
        const res = await axios.get('http://localhost:5283/api/Auth/users')
        users.value = res.data
    } catch (e) {
        console.error("Lỗi lấy danh sách user:", e)
    }
}

const fetchCompanies = async () => {
    if (!isAdmin.value) return
    try {
        const res = await axios.get('http://localhost:5283/api/Companies')
        companies.value = res.data
    } catch (e) {
        console.error("Lỗi lấy danh sách công ty:", e)
    }
}

const openCreateModal = () => {
    modal.value = { show: true, isEdit: false }
    form.value = { username: '', password: '', fullName: '', roleId: 2, companyId: null }
}

const openEditModal = (u) => {
    modal.value = { show: true, isEdit: true }
    form.value = { 
        username: u.username, 
        password: '', 
        fullName: u.fullName || '', 
        roleId: u.roleId, 
        companyId: u.companyId 
    }
}

const handleSubmit = async () => {
    try {
        if (modal.value.isEdit) {
            await axios.put(`http://localhost:5283/api/Auth/users/${form.value.username}`, form.value)
            alert("✅ Cập nhật thành công!")
        } else {
            await axios.post('http://localhost:5283/api/Auth/register', form.value)
            alert("✅ Tạo tài khoản thành công!")
        }
        modal.value.show = false
        fetchUsers()
    } catch (e) {
        alert("❌ Thao tác thất bại. Vui lòng thử lại.")
    }
}

const handleDelete = async (username) => {
    if (!confirm(`Bạn có chắc muốn xóa tài khoản "${username}"?`)) return
    try {
        await axios.delete(`http://localhost:5283/api/Auth/users/${username}`)
        alert("✅ Đã xóa tài khoản!")
        fetchUsers()
    } catch (e) {
        alert("❌ Không thể xóa tài khoản này.")
    }
}

onMounted(async () => {
    if (isAdmin.value) {
        await fetchUsers()
        await fetchCompanies()
    } else {
        await authStore.fetchProfile()
    }
})
</script>

<style scoped>
.animate-fade-in {
  animation: fadeIn 0.8s ease-out;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
