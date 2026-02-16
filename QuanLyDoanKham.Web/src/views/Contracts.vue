<template>
  <div class="space-y-6 animate-fade-in">
    <div class="flex justify-between items-center">
      <h2 class="text-3xl font-black text-slate-800">Quản lý Hợp đồng</h2>
      <button v-if="authStore.role !== 'Customer'" @click="showForm = !showForm" 
              class="bg-primary hover:bg-primary-dark text-white px-6 py-3 rounded-2xl font-bold flex items-center space-x-2 transition-all shadow-lg active:scale-95">
        <Plus class="w-5 h-5" />
        <span>Tạo Hợp đồng mới</span>
      </button>
    </div>

    <!-- Rule: Arrow pointing down to form -->
    <div v-if="showForm" class="flex flex-col items-center animate-bounce mb-2">
        <ArrowDown class="text-primary w-8 h-8" />
    </div>

    <!-- Form Create Contract -->
    <div v-if="showForm" class="bg-white p-8 rounded-[2rem] border border-slate-100 shadow-xl mb-10">
        <h3 class="text-xl font-black mb-6 text-slate-800 flex items-center">
            <div class="w-2 h-6 bg-teal-400 rounded-full mr-3"></div>
            Soạn thảo hợp đồng khám sức khỏe
        </h3>
        <form @submit.prevent="addContract" class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div class="space-y-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Chọn Đối tác</label>
                <select v-model="newContract.companyId" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white outline-none transition-all font-bold">
                    <option v-for="c in companies" :key="c.companyId" :value="c.companyId">{{ c.companyName }}</option>
                </select>
            </div>
            <div class="space-y-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Giá trị hợp đồng (VNĐ)</label>
                <input 
                  type="text" 
                  :value="formattedTotalAmount"
                  @input="handleAmountInput"
                  required 
                  class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white outline-none transition-all font-black text-2xl tracking-widest text-primary" 
                  placeholder="0"
                />
                <p class="text-[10px] font-black text-slate-400 ml-2 uppercase tracking-widest">VNĐ (Đã định dạng tự động)</p>
            </div>
            <div class="space-y-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Số lượng nhân sự khám</label>
                <input 
                  v-model="newContract.patientCount" 
                  type="number" 
                  required 
                  class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white outline-none transition-all font-black text-2xl tracking-widest text-slate-700" 
                  placeholder="0"
                />
            </div>
            <div class="space-y-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Ngày bắt đầu</label>
                <input v-model="newContract.startDate" type="date" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white outline-none transition-all font-bold" />
            </div>
            <div class="space-y-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Ngày kết thúc</label>
                <input v-model="newContract.endDate" type="date" required class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white outline-none transition-all font-bold" />
            </div>
            <div class="flex items-end justify-end space-x-4">
                <button type="submit" class="bg-primary text-white w-full py-4 rounded-2xl font-black shadow-lg hover:bg-primary-dark transition-all">HOÀN TẤT HỢP ĐỒNG</button>
            </div>
        </form>
    </div>

    <!-- Contract List -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div v-for="item in list" :key="item.healthContractId" class="bg-white p-8 rounded-[2.5rem] border border-slate-100 shadow-xl hover:border-primary/20 transition-all group overflow-hidden relative">
            <!-- Two Pluses Icon integration (Concept) -->
            <div class="absolute -right-4 -top-4 w-24 h-24 bg-slate-50 rounded-full flex items-center justify-center pointer-events-none group-hover:bg-primary/5 transition-colors">
                <Plus class="w-4 h-4 text-slate-200 group-hover:text-primary transition-colors" />
                <Plus class="w-6 h-6 text-slate-100 absolute group-hover:text-primary/20 transition-colors" style="transform: translate(12px, 12px)" />
            </div>

            <div class="flex items-center space-x-4 mb-6">
                <div class="bg-teal-50 p-4 rounded-2xl">
                    <FileText class="text-teal-500 w-8 h-8" />
                </div>
                <div>
                    <h4 class="text-xl font-black text-slate-800">{{ item.companyName }}</h4>
                    <p class="text-xs font-bold text-slate-400 uppercase tracking-widest">HĐ: #{{ item.healthContractId }}</p>
                </div>
            </div>

            <div class="grid grid-cols-2 gap-4 mb-6">
                <div class="bg-slate-50 p-4 rounded-2xl">
                    <p class="text-[10px] font-black text-slate-400 uppercase tracking-tighter mb-1">Giá trị</p>
                    <p class="text-lg font-black text-primary">{{ formatPrice(item.totalAmount) }}</p>
                </div>
                <div class="bg-slate-50 p-4 rounded-2xl">
                    <p class="text-[10px] font-black text-slate-400 uppercase tracking-tighter mb-1">Quy mô</p>
                    <p class="text-lg font-black text-slate-700">{{ item.patientCount }} người</p>
                </div>
            </div>

            <div class="flex items-center justify-between">
                <div class="flex items-center space-x-2 text-slate-500 font-bold text-sm">
                    <Calendar class="w-4 h-4" />
                    <span>{{ formatDate(item.startDate) }} - {{ formatDate(item.endDate) }}</span>
                </div>
                <span :class="['px-4 py-1.5 rounded-full text-xs font-black uppercase tracking-widest', item.isFinished ? 'bg-success/10 text-success' : 'bg-warning/10 text-warning']">
                    {{ item.isFinished ? 'Đã xong' : 'Đang chạy' }}
                </span>
            </div>
            
            <button @click="openDetails(item)" class="w-full mt-8 py-4 border-2 border-slate-100 rounded-2xl font-black text-slate-500 hover:bg-slate-900 hover:text-white hover:border-slate-900 transition-all">
                XEM CHI TIẾT
            </button>
        </div>
        
        <div v-if="list.length === 0" class="col-span-2 py-20 text-center text-slate-300 font-bold italic bg-white rounded-[2.5rem] border-2 border-dashed border-slate-100">
            Chưa có hợp đồng nào được tạo
        </div>
    </div>
    <!-- Contract Detail Modal -->
    <div v-if="detailsModal.show" class="fixed inset-0 z-[100] flex items-center justify-center bg-slate-900/80 backdrop-blur-sm p-4">
        <div class="bg-white w-full max-w-2xl p-10 rounded-[2.5rem] shadow-2xl animate-fade-in-up relative overflow-hidden">
            <!-- Decorative Header -->
            <div class="absolute top-0 left-0 right-0 h-32 bg-gradient-to-r from-teal-400 to-teal-600"></div>
            <div class="absolute top-8 left-8 bg-white p-4 rounded-3xl shadow-lg">
                <FileText class="w-10 h-10 text-teal-600" />
            </div>

            <button @click="detailsModal.show = false" class="absolute top-6 right-6 bg-white/20 p-2 rounded-full hover:bg-white/40 transition-all text-white">
                <span class="font-black text-xl">✕</span>
            </button>

            <div class="mt-24 pt-4">
                <div class="flex justify-between items-start mb-8">
                    <div>
                        <h3 class="text-3xl font-black text-slate-800">{{ detailsModal.data.companyName }}</h3>
                        <p class="text-sm font-bold text-slate-400 uppercase tracking-widest mt-1">Mã HĐ: #{{ detailsModal.data.healthContractId }}</p>
                    </div>
                    <span :class="['px-6 py-2 rounded-full text-sm font-black uppercase tracking-widest shadow-sm', detailsModal.data.isFinished ? 'bg-success text-white' : 'bg-warning text-white']">
                        {{ detailsModal.data.isFinished ? 'Đã hoàn thành' : 'Đang thực hiện' }}
                    </span>
                </div>

                <div class="grid grid-cols-2 gap-8 mb-8">
                    <div class="bg-slate-50 p-6 rounded-[2rem]">
                        <p class="text-xs font-black text-slate-400 uppercase tracking-widest mb-2">Tổng giá trị</p>
                        <p class="text-3xl font-black text-primary">{{ formatPrice(detailsModal.data.totalAmount) }}</p>
                    </div>
                    <div class="bg-slate-50 p-6 rounded-[2rem]">
                        <p class="text-xs font-black text-slate-400 uppercase tracking-widest mb-2">Quy mô khám</p>
                        <p class="text-3xl font-black text-slate-700">{{ detailsModal.data.patientCount }} <span class="text-base text-slate-400">nhân sự</span></p>
                    </div>
                </div>

                <div class="space-y-4">
                    <div class="flex items-center space-x-4 p-4 rounded-2xl border border-slate-100">
                        <Calendar class="w-6 h-6 text-slate-400" />
                        <div>
                            <p class="text-[10px] font-black uppercase tracking-widest text-slate-400">Thời gian thực hiện</p>
                            <p class="font-bold text-slate-700 text-lg">
                                {{ formatDate(detailsModal.data.startDate) }} <span class="mx-2 text-slate-300">➔</span> {{ formatDate(detailsModal.data.endDate) }}
                            </p>
                        </div>
                    </div>
                </div>

                <div class="mt-10 flex justify-end">
                    <button @click="detailsModal.show = false" class="bg-slate-100 text-slate-500 hover:bg-slate-200 px-8 py-3 rounded-xl font-black transition-all">ĐÓNG</button>
                </div>
            </div>
        </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { Plus, FileText, Calendar, ArrowDown } from 'lucide-vue-next'
import { useAuthStore } from '../stores/auth'

const authStore = useAuthStore()
const list = ref([])
const companies = ref([])
const showForm = ref(false)
const newContract = ref({
    companyId: null,
    totalAmount: 0,
    patientCount: 0,
    startDate: '',
    endDate: ''
})
const detailsModal = ref({ show: false, data: {} })

const formattedTotalAmount = computed(() => {
    return (newContract.value.totalAmount || 0).toLocaleString('vi-VN')
})

const handleAmountInput = (e) => {
    const val = e.target.value.replace(/\D/g, '')
    newContract.value.totalAmount = val ? parseInt(val) : 0
    e.target.value = (newContract.value.totalAmount || 0).toLocaleString('vi-VN')
}

const fetchList = async () => {
    try {
        const res = await axios.get('http://localhost:5283/api/HealthContracts')
        list.value = res.data
    } catch (e) { 
        console.error(e)
        alert("Không thể tải danh sách hợp đồng. Hãy kiểm tra Backend.")
    }
}

const fetchCompanies = async () => {
    try {
        const res = await axios.get('http://localhost:5283/api/Companies')
        companies.value = res.data
    } catch (e) { console.error(e) }
}

const addContract = async () => {
    try {
        // Validation logic
        const { companyId, totalAmount, patientCount, startDate, endDate } = newContract.value;
        
        if (!companyId) return alert("❌ Lỗi: Bạn chưa chọn đối tác!");
        if (totalAmount <= 0) return alert("❌ Lỗi: Giá trị hợp đồng phải lớn hơn 0!");
        if (patientCount <= 0) return alert("❌ Lỗi: Số lượng nhân sự khám phải lớn hơn 0!");
        if (!startDate) return alert("❌ Lỗi: Bạn chưa chọn ngày bắt đầu!");
        if (!endDate) return alert("❌ Lỗi: Bạn chưa chọn ngày kết thúc!");
        if (new Date(startDate) > new Date(endDate)) return alert("❌ Lỗi: Ngày bắt đầu không thể lớn hơn ngày kết thúc!");

        console.log("Sending contract data:", newContract.value);
        const response = await axios.post('http://localhost:5283/api/HealthContracts', newContract.value)
        
        alert("✅ Chúc mừng: Tạo hợp đồng thành công!");
        fetchList()
        showForm.value = false
        newContract.value = { companyId: null, totalAmount: 0, patientCount: 0, startDate: '', endDate: '' }
    } catch (e) { 
        console.error("API Error Detail:", e.response?.data);
        alert("❌ Lỗi hệ thống: Không thể tạo hợp đồng. Vui lòng kiểm tra lại kết nối Backend.");
    }
}

const openDetails = (contract) => {
    detailsModal.value.data = contract
    detailsModal.value.show = true
}

const formatPrice = (p) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p)
const formatDate = (d) => new Date(d).toLocaleDateString('vi-VN')

onMounted(() => {
    fetchList()
    fetchCompanies()
})
</script>
