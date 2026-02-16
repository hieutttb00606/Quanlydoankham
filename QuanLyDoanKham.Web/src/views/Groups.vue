<template>
  <div class="space-y-10 animate-fade-in pb-20">
    <div class="flex justify-between items-center">
      <h2 class="text-3xl font-black text-slate-800">Vận hành Đoàn khám</h2>
      <button @click="showForm = !showForm" 
              class="bg-amber-400 hover:bg-amber-500 text-white px-6 py-3 rounded-2xl font-bold flex items-center space-x-2 transition-all shadow-lg active:scale-95">
        <Plus class="w-5 h-5" />
        <span>Tạo Đoàn khám mới</span>
      </button>
    </div>

    <!-- Rule: Arrow Down to Form -->
    <div v-if="showForm" class="flex flex-col items-center animate-bounce mb-2">
        <ArrowDown class="text-amber-400 w-8 h-8" />
    </div>

    <!-- Form Create Group -->
    <div v-if="showForm" class="bg-white p-8 rounded-[2rem] border border-amber-100 shadow-xl mb-10">
        <h3 class="text-xl font-black mb-6 text-slate-800 flex items-center">
            <div class="w-2 h-6 bg-amber-400 rounded-full mr-3"></div>
            Thông tin lệnh khám mới
        </h3>
        <form @submit.prevent="addGroup" class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div class="space-y-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Thuộc Hợp đồng</label>
                <select v-model="newGroup.healthContractId" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-amber-400/20 focus:bg-white outline-none transition-all font-bold">
                    <option v-for="c in contracts" :key="c.healthContractId" :value="c.healthContractId">{{ c.companyName }} (#{{c.healthContractId}})</option>
                </select>
            </div>
            <div class="space-y-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Tên đoàn khám</label>
                <input v-model="newGroup.groupName" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-amber-400/20 focus:bg-white outline-none transition-all font-bold" placeholder="VD: Khám khối Văn phòng" />
            </div>
            <div class="space-y-2 text-right flex items-end">
                <div class="flex-1 text-left mr-4">
                    <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Ngày diễn ra</label>
                    <input v-model="newGroup.examDate" type="date" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-amber-400/20 focus:bg-white outline-none transition-all font-bold" />
                </div>
                <button type="submit" class="bg-amber-400 text-white px-8 py-4 rounded-2xl font-black shadow-lg hover:bg-amber-500 transition-all">LƯU LỆNH</button>
            </div>
        </form>
    </div>

    <!-- Groups List with Details -->
    <div class="space-y-12">
        <div v-for="group in groups" :key="group.groupId" class="bg-white rounded-[3rem] border border-slate-100 shadow-xl overflow-hidden">
            <!-- Group Header -->
            <div class="p-8 bg-slate-900 text-white flex justify-between items-center">
                <div class="flex items-center space-x-6">
                    <div class="bg-amber-400 p-4 rounded-3xl">
                        <Stethoscope class="text-white w-8 h-8" />
                    </div>
                    <div>
                        <h4 class="text-2xl font-black tracking-tight">{{ group.groupName }}</h4>
                        <div class="flex items-center space-x-4 mt-1 text-slate-400 text-sm font-bold uppercase tracking-widest">
                            <span class="text-primary">{{ group.companyName }}</span>
                            <span>•</span>
                            <span>{{ formatDate(group.examDate) }}</span>
                        </div>
                    </div>
                </div>
                <div class="text-right">
                    <div class="text-xs text-slate-500 font-black uppercase tracking-tighter mb-1">ID ĐOÀN KHÁM</div>
                    <div class="text-2xl font-black text-amber-400">#{{ group.groupId }}</div>
                </div>
            </div>

            <div class="p-10 grid grid-cols-1 lg:grid-cols-2 gap-12">
                <!-- Logic 1: Nhân sự & Lương (1 củ còn 500k) -->
                <div class="space-y-6 text-slate-800">
                    <div class="flex justify-between items-center">
                        <h5 class="text-lg font-black flex items-center">
                            <Users class="w-5 h-5 mr-3 text-primary" /> Điều phối Nhân sự
                        </h5>
                        <button @click="openAddStaff(group.groupId)" class="text-xs font-black text-primary bg-primary/5 px-4 py-2 rounded-full hover:bg-primary hover:text-white transition-all uppercase tracking-widest">Phân bổ thêm</button>
                    </div>
                    
                    <div class="space-y-3">
                        <div v-for="s in staffDetails[group.groupId]" :key="s.id" class="flex justify-between items-center bg-slate-50 p-4 rounded-2xl hover:bg-blue-50 transition-colors">
                            <div class="flex items-center space-x-4">
                                <div class="w-10 h-10 rounded-full border-2 border-white shadow-sm overflow-hidden">
                                    <img :src="`https://api.dicebear.com/7.x/avataaars/svg?seed=${s.fullName}`" />
                                </div>
                                <div>
                                    <p class="font-black text-base">{{ s.fullName }}</p>
                                    <p class="text-[10px] font-black uppercase tracking-widest text-slate-400">{{ s.jobTitle }} • {{ s.shiftType === 1 ? 'Cả ngày' : 'Sáng/Chiều' }}</p>
                                </div>
                            </div>
                            <div class="text-right">
                                <p class="font-black text-primary">{{ formatPrice(s.calculatedSalary) }}</p>
                                <button @click="removeStaff(s.id, group.groupId)" class="text-[10px] font-extrabold text-rose-300 hover:text-rose-600 uppercase tracking-widest">Xóa</button>
                            </div>
                        </div>
                        <div v-if="!staffDetails[group.groupId]?.length" class="text-center py-6 border-2 border-dashed border-slate-100 rounded-2xl text-slate-300 text-sm font-bold">Chưa phân bổ bác sĩ</div>
                    </div>
                </div>

                <!-- Logic 2: Vật tư & Kho -->
                <div class="space-y-6 text-slate-800">
                    <div class="flex justify-between items-center">
                        <h5 class="text-lg font-black flex items-center">
                            <Package class="w-5 h-5 mr-3 text-amber-500" /> Cấp phát Vật tư
                        </h5>
                        <button @click="openAddSupply(group.groupId)" class="text-xs font-black text-amber-600 bg-amber-50 px-4 py-2 rounded-full hover:bg-amber-500 hover:text-white transition-all uppercase tracking-widest">Xuất từ kho</button>
                    </div>
                    
                    <div class="space-y-3">
                        <div v-for="su in supplyDetails[group.groupId]" :key="su.id" class="flex justify-between items-center bg-amber-50/30 p-4 rounded-2xl">
                            <div>
                                <p class="font-black text-base">{{ su.supplyName }}</p>
                                <p class="text-[10px] font-black uppercase tracking-widest text-amber-500">Dùng: {{ su.quantityUsed }} • Trả: {{ su.returnQuantity }}</p>
                            </div>
                            <div class="flex items-center space-x-4">
                                <button v-if="su.quantityUsed > su.returnQuantity" @click="returnSupplyForm(su, group.groupId)" class="text-[10px] bg-white px-3 py-1.5 rounded-lg border border-amber-200 text-amber-600 font-black hover:bg-amber-500 hover:text-white transition-all uppercase tracking-widest">Hoàn trả</button>
                                <p class="font-black text-slate-800">{{ formatPrice(su.totalCost) }}</p>
                            </div>
                        </div>
                        <div v-if="!supplyDetails[group.groupId]?.length" class="text-center py-6 border-2 border-dashed border-slate-100 rounded-2xl text-slate-300 text-sm font-bold">Chưa xuất vật tư</div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Staff Allocation Modal with Role Filtering -->
    <div v-if="modals.staff.show" class="fixed inset-0 z-[100] flex items-center justify-center bg-slate-900/80 backdrop-blur-sm p-4">
        <div class="bg-white w-full max-w-lg p-10 rounded-[3rem] shadow-2xl animate-fade-in-up">
            <h3 class="text-2xl font-black text-slate-800 mb-6 text-center uppercase tracking-tighter">Phân bổ nhân sự</h3>
            
            <!-- Role Filter Tabs -->
            <div class="flex space-x-2 mb-6 overflow-x-auto">
                <button v-for="role in staffRoles" :key="role" 
                        type="button"
                        @click="selectedRole = role"
                        :class="['px-4 py-2 rounded-xl font-black text-xs whitespace-nowrap transition-all', 
                                 selectedRole === role ? 'bg-primary text-white' : 'bg-slate-100 text-slate-600 hover:bg-slate-200']">
                    {{ role }} ({{ getStaffCountByRole(role) }})
                </button>
            </div>
            
            <form @submit.prevent="addStaffToGroup" class="space-y-6">
                <div>
                  <label class="text-xs font-black uppercase tracking-widest text-slate-400">Chọn Nhân sự</label>
                  <select v-model="modals.staff.data.staffId" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 font-bold border-2 border-transparent focus:border-primary/20 outline-none mt-2">
                    <option value="" disabled>-- Chọn nhân viên --</option>
                    <option v-for="s in filteredStaffList" :key="s.staffId" :value="s.staffId">{{ s.fullName }} ({{s.jobTitle}})</option>
                  </select>
                  <p v-if="filteredStaffList.length === 0" class="text-xs text-rose-500 font-bold mt-2">❌ Không còn nhân sự khả dụng trong vai trò này</p>
                </div>
                <div>
                  <label class="text-xs font-black uppercase tracking-widest text-slate-400">Loại ca làm</label>
                  <select v-model="modals.staff.data.shiftType" class="w-full px-5 py-4 rounded-2xl bg-slate-50 font-bold border-2 border-transparent focus:border-primary/20 outline-none mt-2">
                    <option :value="1.0">Cả ngày (100% lương)</option>
                    <option :value="0.5">Sáng/Chiều (50% lương)</option>
                  </select>
                </div>
                <div class="flex space-x-4 pt-4">
                    <button type="button" @click="closeStaffModal" class="flex-1 font-bold text-slate-400 hover:bg-slate-100 rounded-xl py-3 transition-all">Hủy</button>
                    <button type="submit" :disabled="!modals.staff.data.staffId" class="flex-[2] bg-primary text-white py-4 rounded-2xl font-black shadow-lg shadow-primary/20 disabled:opacity-50 disabled:cursor-not-allowed">XÁC NHẬN</button>
                </div>
            </form>
        </div>
    </div>

    <!-- Supply Export Modal -->
    <div v-if="modals.supply.show" class="fixed inset-0 z-[100] flex items-center justify-center bg-slate-900/80 backdrop-blur-sm p-4">
        <div class="bg-white w-full max-w-md p-10 rounded-[3rem] shadow-2xl animate-fade-in-up">
            <h3 class="text-2xl font-black text-slate-800 mb-8 text-center uppercase tracking-tighter">Xuất vật tư từ kho</h3>
            <form @submit.prevent="addSupplyToGroup" class="space-y-6">
                <div>
                  <label class="text-xs font-black uppercase tracking-widest text-slate-400">Chọn Vật tư</label>
                  <select v-model="modals.supply.data.supplyId" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 font-bold border-2 border-transparent focus:border-amber-400/20 outline-none mt-2">
                    <option value="" disabled>-- Chọn vật tư --</option>
                    <option v-for="s in supplyList" :key="s.supplyId" :value="s.supplyId">{{ s.supplyName }} (Còn: {{s.stockQuantity}})</option>
                  </select>
                </div>
                <div>
                  <label class="text-xs font-black uppercase tracking-widest text-slate-400">Số lượng xuất</label>
                  <input v-model="modals.supply.data.quantityUsed" type="number" min="1" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 font-bold border-2 border-transparent focus:border-amber-400/20 outline-none mt-2" />
                </div>
                <div class="flex space-x-4 pt-4">
                    <button type="button" @click="modals.supply.show = false" class="flex-1 font-bold text-slate-400 hover:bg-slate-100 rounded-xl py-3 transition-all">Hủy</button>
                    <button type="submit" :disabled="!modals.supply.data.supplyId" class="flex-[2] bg-amber-500 text-white py-4 rounded-2xl font-black shadow-lg shadow-amber-200 disabled:opacity-50 disabled:cursor-not-allowed">XUẤT KHO</button>
                </div>
            </form>
        </div>
    </div>

    <!-- Supply Return Modal -->
    <div v-if="modals.returnSupply.show" class="fixed inset-0 z-[100] flex items-center justify-center bg-slate-900/80 backdrop-blur-sm p-4">
        <div class="bg-white w-full max-w-md p-10 rounded-[3rem] shadow-2xl animate-fade-in-up">
            <h3 class="text-2xl font-black text-slate-800 mb-6 text-center uppercase tracking-tighter">Hoàn trả vật tư</h3>
            <div class="bg-amber-50 p-6 rounded-2xl mb-6">
                <p class="font-black text-lg text-slate-800 mb-2">{{ modals.returnSupply.supply?.supplyName }}</p>
                <div class="flex justify-between text-sm font-bold text-slate-600">
                    <span>Đã dùng:</span>
                    <span class="text-amber-600">{{ modals.returnSupply.supply?.quantityUsed }}</span>
                </div>
                <div class="flex justify-between text-sm font-bold text-slate-600">
                    <span>Đã trả:</span>
                    <span class="text-success">{{ modals.returnSupply.supply?.returnQuantity }}</span>
                </div>
                <div class="flex justify-between text-sm font-bold text-slate-800 border-t border-amber-200 mt-2 pt-2">
                    <span>Còn có thể trả:</span>
                    <span class="text-primary">{{ modals.returnSupply.supply?.quantityUsed - modals.returnSupply.supply?.returnQuantity }}</span>
                </div>
            </div>
            <form @submit.prevent="confirmReturn" class="space-y-6">
                <div>
                  <label class="text-xs font-black uppercase tracking-widest text-slate-400">Số lượng hoàn trả</label>
                  <input v-model="modals.returnSupply.returnQty" type="number" min="1" :max="modals.returnSupply.supply?.quantityUsed - modals.returnSupply.supply?.returnQuantity" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 font-bold border-2 border-success/20 outline-none mt-2 text-2xl text-success" />
                </div>
                <div class="flex space-x-4 pt-4">
                    <button type="button" @click="modals.returnSupply.show = false" class="flex-1 font-bold text-slate-400 hover:bg-slate-100 rounded-xl py-3 transition-all">Hủy</button>
                    <button type="submit" class="flex-[2] bg-success text-white py-4 rounded-2xl font-black shadow-lg shadow-success/20">XÁC NHẬN</button>
                </div>
            </form>
        </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { Plus, ArrowDown, Stethoscope, Users, Package } from 'lucide-vue-next'
import { useToast } from '../composables/useToast'

const toast = useToast()

const groups = ref([])
const contracts = ref([])
const staffList = ref([])
const supplyList = ref([])
const showForm = ref(false)

// Details for each group
const staffDetails = ref({})
const supplyDetails = ref({})

const newGroup = ref({ healthContractId: null, groupName: '', examDate: new Date().toISOString().split('T')[0] })

// Add watcher for healthContractId
import { watch } from 'vue'
watch(() => newGroup.value.healthContractId, (newId) => {
    if (newId) {
        const selected = contracts.value.find(c => c.healthContractId === newId)
        if (selected) {
            newGroup.value.groupName = `Khám sức khỏe - ${selected.companyName}`
            // Auto fill date from contract start date
            if (selected.startDate) {
                newGroup.value.examDate = selected.startDate.split('T')[0]
            }
        }
    }
})

const modals = ref({
    staff: { show: false, groupId: null, data: { staffId: '', shiftType: 1.0 } },
    supply: { show: false, groupId: null, data: { supplyId: '', quantityUsed: 1 } },
    returnSupply: { show: false, supply: null, returnQty: 0 }
})

// Staff role filtering
const staffRoles = ref(['Tất cả', 'Bác sĩ', 'Điều dưỡng', 'Kỹ thuật viên'])
const selectedRole = ref('Tất cả')

// Filtered staff list based on role and availability
const filteredStaffList = computed(() => {
    const groupId = modals.value.staff.groupId
    if (!groupId) return []
    
    const allocatedStaffIds = (staffDetails.value[groupId] || []).map(s => s.staffId)
    
    return staffList.value.filter(s => {
        // Filter by role
        if (selectedRole.value !== 'Tất cả' && s.jobTitle !== selectedRole.value) return false
        
        // Filter out already allocated staff
        return !allocatedStaffIds.includes(s.staffId)
    })
})

// Get staff count by role (for tab badges)
const getStaffCountByRole = (role) => {
    const groupId = modals.value.staff.groupId
    if (!groupId) return 0
    
    const allocatedStaffIds = (staffDetails.value[groupId] || []).map(s => s.staffId)
    
    if (role === 'Tất cả') {
        return staffList.value.filter(s => !allocatedStaffIds.includes(s.staffId)).length
    }
    
    return staffList.value.filter(s => 
        s.jobTitle === role && !allocatedStaffIds.includes(s.staffId)
    ).length
}

const fetchData = async () => {
    try {
        const [gRes, cRes, sListRes, suListRes] = await Promise.all([
            axios.get('http://localhost:5283/api/MedicalGroups'),
            axios.get('http://localhost:5283/api/HealthContracts'),
            axios.get('http://localhost:5283/api/Staffs'),
            axios.get('http://localhost:5283/api/Supplies')
        ])
        groups.value = gRes.data
        contracts.value = cRes.data
        staffList.value = sListRes.data
        supplyList.value = suListRes.data
        
        // Fetch details for each group
        groups.value.forEach(g => {
            fetchStaffDetails(g.groupId)
            fetchSupplyDetails(g.groupId)
        })
    } catch (e) { console.error(e) }
}

const fetchStaffDetails = async (gid) => {
    const res = await axios.get(`http://localhost:5283/api/GroupDetails/staff/${gid}`)
    staffDetails.value[gid] = res.data
}

const fetchSupplyDetails = async (gid) => {
    const res = await axios.get(`http://localhost:5283/api/GroupDetails/supply/${gid}`)
    supplyDetails.value[gid] = res.data
}

const addGroup = async () => {
    await axios.post('http://localhost:5283/api/MedicalGroups', newGroup.value)
    showForm.value = false
    fetchData()
}

const openAddStaff = (gid) => { 
    modals.value.staff.groupId = gid
    modals.value.staff.data = { staffId: '', shiftType: 1.0 }
    selectedRole.value = 'Tất cả'
    modals.value.staff.show = true 
}

const openAddSupply = (gid) => { 
    modals.value.supply.groupId = gid
    modals.value.supply.data = { supplyId: '', quantityUsed: 1 }
    modals.value.supply.show = true 
}

const closeStaffModal = () => {
    modals.value.staff.show = false
    modals.value.staff.data = { staffId: '', shiftType: 1.0 }
    selectedRole.value = 'Tất cả'
}

const addStaffToGroup = async () => {
    try {
        const gid = modals.value.staff.groupId
        await axios.post('http://localhost:5283/api/GroupDetails/staff', { ...modals.value.staff.data, groupId: gid })
        toast.success('Đã phân bổ nhân sự thành công!')
        closeStaffModal()
        fetchStaffDetails(gid)
    } catch (e) {
        console.error(e)
        const errorMsg = e.response?.data || 'Không thể phân bổ nhân sự'
        toast.error(errorMsg)
    }
}

const addSupplyToGroup = async () => {
    try {
        const gid = modals.value.supply.groupId
        const supplyId = modals.value.supply.data.supplyId
        const qty = modals.value.supply.data.quantityUsed
        
        // Front-end Validation
        const selectedSupply = supplyList.value.find(s => s.supplyId === supplyId)
        if (selectedSupply && selectedSupply.stockQuantity < qty) {
            toast.warning(`Số lượng trong kho không đủ! (Hiện có: ${selectedSupply.stockQuantity})`)
            return
        }

        await axios.post('http://localhost:5283/api/GroupDetails/supply', { ...modals.value.supply.data, groupId: gid })
        toast.success('Xuất vật tư thành công!')
        modals.value.supply.show = false
        fetchSupplyDetails(gid)
        fetchData()
    } catch (e) {
        console.error(e)
        const errorMsg = e.response?.data || "Không thể xuất kho"
        toast.error(errorMsg)
    }
}

const returnSupplyForm = (supply, groupId) => {
    modals.value.returnSupply = {
        show: true,
        supply: { ...supply, groupId },
        returnQty: 1
    }
}

const confirmReturn = async () => {
    try {
        const { supply, returnQty } = modals.value.returnSupply
        const maxReturn = supply.quantityUsed - supply.returnQuantity
        
        if (returnQty > maxReturn) {
            toast.warning(`Chỉ có thể trả tối đa ${maxReturn} ${supply.supplyName}`)
            return
        }
        
        await axios.put(`http://localhost:5283/api/GroupDetails/supply/${supply.id}/return`, { returnQuantity: returnQty })
        toast.success(`Đã hoàn trả ${returnQty} ${supply.supplyName}`)
        modals.value.returnSupply.show = false
        fetchSupplyDetails(supply.groupId)
        fetchData()
    } catch (e) {
        console.error(e)
        const errorMsg = e.response?.data || 'Không thể hoàn trả vật tư'
        toast.error(errorMsg)
    }
}

const removeStaff = async (id, gid) => {
    try {
        await axios.delete(`http://localhost:5283/api/GroupDetails/staff/${id}`)
        toast.success('Đã xóa nhân sự khỏi đoàn khám')
        fetchStaffDetails(gid)
    } catch (e) {
        console.error(e)
        toast.error('Không thể xóa nhân sự')
    }
}

const formatPrice = (p) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p)
const formatDate = (d) => new Date(d).toLocaleDateString('vi-VN')

onMounted(fetchData)
</script>
