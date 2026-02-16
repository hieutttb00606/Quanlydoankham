<template>
  <div class="space-y-6 animate-fade-in pb-20">
    <!-- Header Section -->
    <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 text-slate-800">
      <div>
        <h2 class="text-3xl font-black tracking-tight flex items-center gap-3">
          <Users class="w-8 h-8 text-indigo-600" />
          Quản lý Nhân sự Y tế
        </h2>
        <p class="text-slate-500 font-medium">Hệ thống quản lý thông tin bác sĩ, điều dưỡng và kỹ thuật viên</p>
      </div>
      <button @click="openModal()" 
              class="bg-indigo-600 hover:bg-indigo-700 text-white px-8 py-4 rounded-2xl font-black flex items-center space-x-3 transition-all shadow-xl shadow-indigo-200 active:scale-95 group">
        <Plus class="w-6 h-6 group-hover:rotate-90 transition-transform" />
        <span>Thêm Nhân sự mới</span>
      </button>
    </div>

    <!-- Stats & Filters -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div v-for="stat in stats" :key="stat.label" class="bg-white p-6 rounded-3xl border border-slate-100 shadow-sm">
        <div class="text-xs font-black uppercase tracking-widest text-slate-400 mb-2">{{ stat.label }}</div>
        <div class="text-3xl font-black text-slate-800">{{ stat.value }}</div>
      </div>
    </div>

    <!-- Search & Tabs -->
    <div class="flex flex-col md:flex-row justify-between items-center gap-6 bg-white p-4 rounded-[2rem] shadow-sm border border-slate-100">
        <div class="flex flex-wrap gap-2 bg-slate-50 p-1.5 rounded-2xl w-fit">
            <button v-for="tab in tabs" :key="tab.value"
                    @click="activeTab = tab.value"
                    :class="['px-6 py-2.5 rounded-xl font-black text-sm transition-all', 
                             activeTab === tab.value ? 'bg-white text-indigo-600 shadow-sm' : 'text-slate-400 hover:text-slate-600']">
                {{ tab.label }}
            </button>
        </div>
        <div class="relative w-full md:w-96">
            <Search class="absolute left-5 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
            <input v-model="searchQuery" placeholder="Tìm tên, mã NV, số CMND..." 
                   class="w-full pl-14 pr-6 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-indigo-500/10 focus:bg-white outline-none transition-all font-bold" />
        </div>
    </div>

    <!-- Staff Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div v-for="item in filteredList" :key="item.staffId" 
             class="bg-white p-8 rounded-[2.5rem] border border-slate-100 shadow-sm hover:shadow-2xl hover:shadow-indigo-100/50 transition-all relative overflow-hidden group cursor-pointer"
             @click="openModal(item)">
            
            <div class="flex items-start justify-between mb-6">
                <div class="flex items-center space-x-4">
                    <div class="w-16 h-16 rounded-2xl bg-gradient-to-br from-indigo-50 to-blue-50 flex items-center justify-center overflow-hidden border border-indigo-100 shadow-inner">
                        <img :src="`https://api.dicebear.com/7.x/avataaars/svg?seed=${item.fullName}`" alt="avatar" class="w-12 h-12" />
                    </div>
                    <div>
                        <div class="text-[10px] font-black tracking-[0.2em] text-indigo-400 uppercase mb-0.5">{{ item.employeeCode || 'CHƯA CẬP NHẬT' }}</div>
                        <h4 class="font-black text-slate-800 text-lg leading-tight">{{ item.fullName }}</h4>
                        <p class="text-xs font-bold text-slate-400">{{ item.jobTitle }} • {{ item.employeeType || 'Chưa phân loại' }}</p>
                    </div>
                </div>
                <div class="bg-indigo-50 text-indigo-600 p-2 rounded-xl opacity-0 group-hover:opacity-100 transition-opacity">
                    <ChevronRight class="w-5 h-5" />
                </div>
            </div>

            <div class="space-y-3 border-t border-slate-50 pt-6">
                <div class="flex justify-between items-center">
                    <span class="text-xs font-bold text-slate-400">Số điện thoại</span>
                    <span class="text-sm font-black text-slate-700">{{ item.phoneNumber || '---' }}</span>
                </div>
                <div class="flex justify-between items-center">
                    <span class="text-xs font-bold text-slate-400">CMND/CCCD</span>
                    <span class="text-sm font-black text-slate-700">{{ item.idCardNumber || '---' }}</span>
                </div>
                <div class="mt-4 p-4 rounded-2xl bg-slate-50/50 flex justify-between items-center">
                    <div class="text-xs font-bold text-slate-400 uppercase tracking-widest">Lương/Buổi</div>
                    <div class="font-black text-indigo-600 text-lg">{{ formatPrice(item.baseSalary) }}</div>
                </div>
            </div>

            <!-- Accent line -->
            <div class="absolute bottom-0 left-0 h-1.5 bg-indigo-500 transition-all duration-500" :style="{ width: '0%', groupHover: { width: '100%' } }"></div>
        </div>
        
        <div v-if="filteredList.length === 0" class="col-span-full py-32 text-center bg-white rounded-[3rem] border-4 border-dashed border-slate-50">
            <div class="flex flex-col items-center gap-4">
                <div class="w-20 h-20 bg-slate-50 rounded-full flex items-center justify-center">
                    <SearchOff class="w-10 h-10 text-slate-200" />
                </div>
                <div>
                    <p class="text-xl font-black text-slate-300">Không tìm thấy nhân viên nào</p>
                    <p class="text-slate-400 font-medium">Thử thay đổi bộ lọc hoặc từ tìm kiếm khác</p>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal Form -->
    <div v-if="showModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 md:p-10 backdrop-blur-xl bg-slate-900/40 animate-fade-in">
        <div class="bg-white w-full max-w-5xl max-h-[90vh] overflow-hidden rounded-[3rem] shadow-2xl flex flex-col animate-scale-up">
            <!-- Modal Header -->
            <div class="p-8 border-b border-slate-100 flex justify-between items-center bg-white sticky top-0 z-10">
                <div class="flex items-center gap-4">
                    <div class="w-12 h-12 bg-indigo-600 text-white rounded-2xl flex items-center justify-center shadow-lg shadow-indigo-200">
                        <UserPlus v-if="!currentStaff.staffId" class="w-6 h-6" />
                        <UserCheck v-else class="w-6 h-6" />
                    </div>
                    <div>
                        <h3 class="text-2xl font-black text-slate-800">{{ currentStaff.staffId ? 'Cập nhật thông tin' : 'Thêm Nhân sự mới' }}</h3>
                        <p class="text-sm font-medium text-slate-500">Mã nhân viên: <span class="text-indigo-600 font-bold">{{ currentStaff.employeeCode || 'TỰ ĐỘNG' }}</span></p>
                    </div>
                </div>
                <button @click="showModal = false" class="p-4 hover:bg-slate-50 rounded-2xl transition-all">
                    <X class="w-6 h-6 text-slate-400" />
                </button>
            </div>

            <!-- Modal Body -->
            <div class="p-8 overflow-y-auto custom-scrollbar bg-slate-50/30">
                <form id="staffForm" @submit.prevent="saveStaff" class="space-y-10">
                    <!-- section 1: Basic Info -->
                    <div class="space-y-6">
                        <div class="flex items-center gap-3">
                            <div class="w-1 h-6 bg-indigo-500 rounded-full"></div>
                            <h4 class="text-sm font-black uppercase tracking-[0.2em] text-slate-400">Thông tin cá nhân</h4>
                        </div>
                        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Họ và Tên (Có dấu) *</label>
                                <input v-model="currentStaff.fullName" required placeholder="Nguyễn Văn A" 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Họ và Tên (Không dấu)</label>
                                <input v-model="currentStaff.fullNameUnsigned" placeholder="NGUYEN VAN A" 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Mã Nhân Viên</label>
                                <input v-model="currentStaff.employeeCode" placeholder="NV001" 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Năm sinh</label>
                                <input v-model.number="currentStaff.birthYear" type="number" placeholder="1990" 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Giới tính</label>
                                <select v-model="currentStaff.gender" class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold">
                                    <option value="Nam">Nam</option>
                                    <option value="Nữ">Nữ</option>
                                    <option value="Khác">Khác</option>
                                </select>
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Số CMND/CCCD</label>
                                <input v-model="currentStaff.idCardNumber" maxlength="20" placeholder="001..." 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Số Điện Thoại</label>
                                <input v-model="currentStaff.phoneNumber" placeholder="090..." 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                        </div>
                    </div>

                    <!-- section 2: Work & Financial -->
                    <div class="space-y-6">
                        <div class="flex items-center gap-3">
                            <div class="w-1 h-6 bg-blue-500 rounded-full"></div>
                            <h4 class="text-sm font-black uppercase tracking-[0.2em] text-slate-400">Công việc & Tài chính</h4>
                        </div>
                        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Chức danh</label>
                                <select v-model="currentStaff.jobTitle" class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold">
                                    <option value="Bác sĩ">Bác sĩ</option>
                                    <option value="Điều dưỡng">Điều dưỡng</option>
                                    <option value="Kỹ thuật viên">Kỹ thuật viên</option>
                                    <option value="Khác">Khác</option>
                                </select>
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Đơn vị / Phòng ban</label>
                                <input v-model="currentStaff.department" placeholder="Nội tổng quát..." 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Loại nhân sự</label>
                                <select v-model="currentStaff.employeeType" class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold">
                                    <option value="Nội bộ">Nội bộ</option>
                                    <option value="Thuê ngoài">Thuê ngoài</option>
                                </select>
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Lương cơ bản (1 buổi) *</label>
                                <div class="relative">
                                    <input 
                                        type="text" 
                                        :value="currentStaff.baseSalary?.toLocaleString('vi-VN')" 
                                        @input="e => {
                                            const val = e.target.value.replace(/\D/g, '');
                                            currentStaff.baseSalary = val ? parseInt(val) : 0;
                                            e.target.value = currentStaff.baseSalary.toLocaleString('vi-VN');
                                        }"
                                        required 
                                        class="w-full pl-6 pr-14 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-black text-indigo-600 text-lg" 
                                        placeholder="1.000.000" 
                                    />
                                    <span class="absolute right-6 top-1/2 -translate-y-1/2 text-xs font-black text-slate-400">VNĐ</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- section 3: Banking -->
                    <div class="space-y-6">
                        <div class="flex items-center gap-3">
                            <div class="w-1 h-6 bg-emerald-500 rounded-full"></div>
                            <h4 class="text-sm font-black uppercase tracking-[0.2em] text-slate-400">Thông tin tài khoản ngân hàng</h4>
                        </div>
                        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Số tài khoản</label>
                                <input v-model="currentStaff.bankAccountNumber" placeholder="123456..." 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Tên chủ tài khoản</label>
                                <input v-model="currentStaff.bankAccountName" placeholder="VIET NAM A" 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Tên ngân hàng</label>
                                <input v-model="currentStaff.bankName" placeholder="VCB, Techcombank..." 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                            <div class="space-y-2">
                                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 px-1">Mã số thuế</label>
                                <input v-model="currentStaff.taxCode" placeholder="8...123" 
                                       class="w-full px-6 py-4 rounded-2xl border-2 border-white bg-white shadow-sm focus:border-indigo-500/20 outline-none transition-all font-bold placeholder:text-slate-300" />
                            </div>
                        </div>
                    </div>
                </form>
            </div>

            <!-- Modal Footer -->
            <div class="p-8 border-t border-slate-100 flex justify-between items-center bg-white sticky bottom-0 z-10 gap-4">
                <button v-if="currentStaff.staffId" @click="removeItem" type="button" 
                        class="text-red-500 font-bold px-6 py-4 rounded-2xl hover:bg-red-50 transition-all flex items-center gap-2">
                    <Trash2 class="w-5 h-5" />
                    Xóa nhân viên
                </button>
                <div class="flex-1"></div>
                <button @click="showModal = false" type="button" class="px-8 py-4 rounded-2xl font-black text-slate-400 hover:bg-slate-50 transition-all">Hủy</button>
                <button form="staffForm" type="submit" 
                        class="bg-indigo-600 text-white px-12 py-4 rounded-2xl font-black shadow-lg shadow-indigo-100 hover:bg-indigo-700 transition-all flex items-center gap-3 active:scale-95">
                    <Save class="w-5 h-5" />
                    {{ currentStaff.staffId ? 'Lưu thay đổi' : 'Tạo nhân viên' }}
                </button>
            </div>
        </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { 
  Users, Plus, Search, ChevronRight, X, Save, Trash2, 
  UserPlus, UserCheck, SearchX as SearchOff 
} from 'lucide-vue-next'

const list = ref([])
const showModal = ref(false)
const activeTab = ref('All')
const searchQuery = ref('')
const currentStaff = ref({})

const stats = computed(() => [
  { label: 'Tổng nhân sự', value: list.value.length },
  { label: 'Bác sĩ', value: list.value.filter(s => s.jobTitle === 'Bác sĩ').length },
  { label: 'Điều dưỡng', value: list.value.filter(s => s.jobTitle === 'Điều dưỡng').length },
  { label: 'Kỹ thuật viên', value: list.value.filter(s => s.jobTitle === 'Kỹ thuật viên').length }
])

const tabs = [
    { label: 'Tất cả', value: 'All' },
    { label: 'Bác sĩ', value: 'Bác sĩ' },
    { label: 'Điều dưỡng', value: 'Điều dưỡng' },
    { label: 'Kỹ thuật viên', value: 'Kỹ thuật viên' }
]

const filteredList = computed(() => {
    let result = list.value
    
    if (activeTab.value !== 'All') {
        result = result.filter(s => s.jobTitle === activeTab.value)
    }
    
    if (searchQuery.value) {
        const q = searchQuery.value.toLowerCase()
        result = result.filter(s => 
            s.fullName?.toLowerCase().includes(q) || 
            s.employeeCode?.toLowerCase().includes(q) || 
            s.idCardNumber?.includes(q) ||
            s.phoneNumber?.includes(q)
        )
    }
    
    return result
})

const fetchList = async () => {
    try {
        const res = await axios.get('http://localhost:5283/api/Staffs')
        list.value = res.data
    } catch (e) { console.error(e) }
}

const openModal = (staff = null) => {
    if (staff) {
        currentStaff.value = { ...staff }
    } else {
        currentStaff.value = {
            fullName: '',
            jobTitle: 'Bác sĩ',
            employeeType: 'Nội bộ',
            gender: 'Nam',
            baseSalary: 1000000
        }
    }
    showModal.value = true
}

const saveStaff = async () => {
    try {
        const staff = { ...currentStaff.value };
        
        // Validation logic
        if (staff.idCardNumber && !/^\d{9,12}$/.test(staff.idCardNumber)) {
            return alert("❌ Lỗi: Số CMND/CCCD phải là số và có độ dài từ 9 đến 12 ký tự!");
        }
        if (staff.phoneNumber && !/^0\d{9,10}$/.test(staff.phoneNumber)) {
            return alert("❌ Lỗi: Số điện thoại không đúng định dạng (10-11 số, bắt đầu bằng 0)!");
        }

        if (staff.staffId) {
            await axios.put(`http://localhost:5283/api/Staffs/${staff.staffId}`, staff)
            alert("✅ Thành công: Đã cập nhật thông tin nhân viên!");
        } else {
            await axios.post('http://localhost:5283/api/Staffs', staff)
            alert("✅ Thành công: Đã tạo nhân viên mới!");
        }
        
        showModal.value = false
        fetchList()
    } catch (e) { 
        console.error(e)
        alert("❌ Lỗi: Không thể lưu thông tin nhân viên.")
    }
}

const removeItem = async () => {
    if (!confirm(`Bạn có chắc muốn xóa nhân viên ${currentStaff.value.fullName}?`)) return
    
    try {
        await axios.delete(`http://localhost:5283/api/Staffs/${currentStaff.value.staffId}`)
        alert("✅ Đã xóa nhân viên thành công!")
        showModal.value = false
        fetchList()
    } catch (e) {
        console.error(e)
        alert("❌ Lỗi: Không thể xóa nhân viên.")
    }
}

const formatPrice = (p) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p)

onMounted(fetchList)
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 8px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #e2e8f0;
  border-radius: 10px;
}
.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: #cbd5e1;
}

@keyframes fade-in {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes scale-up {
  from { opacity: 0; transform: scale(0.95); }
  to { opacity: 1; transform: scale(1); }
}

.animate-fade-in {
  animation: fade-in 0.4s ease-out forwards;
}

.animate-scale-up {
  animation: scale-up 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}
</style>
