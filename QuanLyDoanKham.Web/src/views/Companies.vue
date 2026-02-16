<template>
  <div class="space-y-6 animate-fade-in">
    <!-- Header Actions -->
    <div class="flex justify-between items-center">
      <h2 class="text-3xl font-black text-slate-800">Đối tác Doanh nghiệp</h2>
      <button @click="showForm = !showForm" :disabled="isLoading"
              class="bg-primary hover:bg-primary-dark text-white px-6 py-3 rounded-2xl font-bold flex items-center space-x-2 transition-all shadow-lg active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed">
        <Plus class="w-5 h-5" />
        <span>Thêm Đối tác mới</span>
      </button>
    </div>

    <!-- Rule: Arrow pointing to form -->
    <div v-if="showForm" class="flex flex-col items-center animate-bounce mb-2">
        <ArrowDown class="text-primary w-8 h-8" />
    </div>

    <!-- Form Add/Edit Company -->
    <div v-if="showForm || editingCompany" class="bg-white p-8 rounded-[2rem] border border-slate-100 shadow-xl search-shadow mb-10 overflow-hidden relative">
        <h3 class="text-xl font-black mb-6 text-slate-800 flex items-center">
            <div class="w-2 h-6 bg-accent rounded-full mr-3"></div>
            {{ editingCompany ? 'Chỉnh sửa thông tin công ty' : 'Thông tin công ty mới' }}
        </h3>
        <form @submit.prevent="editingCompany ? updateCompany() : addCompany()" class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="space-y-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Tên Công ty</label>
                <input v-model="currentCompany.companyName" required :disabled="isLoading" class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white outline-none transition-all font-bold disabled:opacity-50" />
            </div>
            <div class="space-y-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Mã số thuế</label>
                <input v-model="currentCompany.taxCode" required :disabled="isLoading" class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white outline-none transition-all font-bold disabled:opacity-50" />
            </div>
            <div class="space-y-2 md:col-span-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Địa chỉ</label>
                <input v-model="currentCompany.address" required :disabled="isLoading" class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white outline-none transition-all font-bold disabled:opacity-50" />
            </div>
            <div class="space-y-2">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Số điện thoại (10 số)</label>
                <div class="relative group">
                  <input 
                    type="text"
                    maxlength="12"
                    @input="handlePhoneInput"
                    required 
                    :disabled="isLoading"
                    class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white outline-none transition-all font-black text-2xl tracking-widest text-primary disabled:opacity-50" 
                    placeholder="0xxx.xxx.xxx"
                  />
                  <div class="absolute right-4 top-1/2 -translate-y-1/2 text-[10px] font-black bg-white px-2 py-1 rounded-lg border border-slate-100 shadow-sm" :class="currentCompany.phoneNumber.length === 10 ? 'text-success border-success/20' : 'text-slate-300'">
                    {{ currentCompany.phoneNumber.length }}/10
                  </div>
                </div>
                <p v-if="currentCompany.phoneNumber.length > 0" class="text-sm font-black text-primary ml-2 animate-pulse flex items-center">
                   <Phone class="w-3 h-3 mr-2" /> {{ formatPhone(currentCompany.phoneNumber) }}
                </p>
            </div>
            <div class="flex items-end justify-end space-x-4">
                <button type="button" @click="cancelEdit" :disabled="isLoading" class="px-6 py-4 text-slate-400 font-bold hover:bg-slate-50 rounded-xl transition-all disabled:opacity-50">Hủy bỏ</button>
                <button type="submit" :disabled="isLoading" class="bg-slate-900 text-white px-10 py-4 rounded-2xl font-black shadow-lg hover:bg-black transition-all disabled:opacity-50 flex items-center space-x-2">
                    <Loader v-if="isLoading" class="w-5 h-5 animate-spin" />
                    <span>{{ isLoading ? 'ĐANG XỬ LÝ...' : (editingCompany ? 'CẬP NHẬT' : 'LƯU THÔNG TIN') }}</span>
                </button>
            </div>
        </form>
    </div>

    <!-- Statistics Grid -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white p-6 rounded-3xl border border-slate-100 shadow-sm">
            <p class="text-slate-400 text-xs font-black uppercase tracking-widest mb-1">Tổng đối tác</p>
            <p class="text-3xl font-black text-slate-900">{{ filteredList.length }}</p>
        </div>
        <div class="bg-white p-6 rounded-3xl border border-slate-100 shadow-sm">
            <p class="text-slate-400 text-xs font-black uppercase tracking-widest mb-1">Đang hiển thị</p>
            <p class="text-3xl font-black text-primary">{{ filteredList.length }}</p>
        </div>
    </div>

    <!-- Table List -->
    <div class="bg-white rounded-[2.5rem] border border-slate-100 shadow-xl overflow-hidden">
        <div class="p-8 border-b border-slate-50 flex justify-between items-center bg-slate-50/50">
            <h3 class="font-black text-slate-800 tracking-tight">DANH SÁCH ĐỐI TÁC</h3>
            <div class="relative">
                <Search class="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
                <input v-model="searchQuery" placeholder="Tìm nhanh..." class="pl-10 pr-4 py-2 bg-white rounded-full border border-slate-200 text-sm outline-none focus:ring-2 focus:ring-primary/20 font-bold" />
            </div>
        </div>
        <table class="w-full text-left border-collapse">
            <thead>
                <tr class="text-xs font-black text-slate-400 uppercase tracking-widest border-b border-slate-50">
                    <th class="px-8 py-6">Tên Công ty</th>
                    <th class="px-8 py-6">MST</th>
                    <th class="px-8 py-6">Liên hệ</th>
                    <th class="px-8 py-6 text-right">Thao tác</th>
                </tr>
            </thead>
            <tbody class="divide-y divide-slate-50">
                <tr v-for="item in filteredList" :key="item.companyId" class="hover:bg-slate-50/80 transition-colors group">
                    <td class="px-8 py-6">
                        <div class="font-black text-slate-800 text-lg">{{ item.companyName }}</div>
                        <div class="text-xs font-medium text-slate-400 flex items-center mt-1">
                            <MapPin class="w-3 h-3 mr-1" /> {{ item.address }}
                        </div>
                    </td>
                    <td class="px-8 py-6">
                        <div class="inline-flex items-center px-3 py-1 rounded-lg bg-slate-100 text-slate-700 font-bold text-sm">
                            # {{ item.taxCode }}
                        </div>
                    </td>
                    <td class="px-8 py-6">
                        <div class="flex items-center text-primary font-black text-lg tracking-tight">
                            <Phone class="w-5 h-5 mr-2 opacity-50" /> {{ formatPhone(item.phoneNumber) }}
                        </div>
                    </td>
                    <td class="px-8 py-6 text-right">
                        <div class="flex justify-end space-x-2 opacity-0 group-hover:opacity-100 transition-opacity">
                            <button @click="editCompany(item)" class="bg-indigo-50 text-indigo-600 p-3 rounded-xl hover:bg-indigo-100 transition-all"><Edit2 class="w-4 h-4" /></button>
                            <button @click="confirmDelete(item)" class="bg-rose-50 text-rose-600 p-3 rounded-xl hover:bg-rose-100 transition-all"><Trash2 class="w-4 h-4" /></button>
                        </div>
                    </td>
                </tr>
                <tr v-if="filteredList.length === 0">
                    <td colspan="4" class="px-8 py-20 text-center text-slate-300 font-bold italic">
                        {{ searchQuery ? 'Không tìm thấy kết quả phù hợp' : 'Chưa có dữ liệu đối tác' }}
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <!-- Confirm Delete Dialog -->
    <ConfirmDialog 
      v-model="showDeleteConfirm"
      title="Xóa công ty?"
      :message="`Bạn có chắc muốn xóa &quot;${companyToDelete?.companyName}&quot;? Hành động này không thể hoàn tác.`"
      confirmText="Xóa ngay"
      variant="danger"
      @confirm="deleteCompany"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { Plus, Search, MapPin, Phone, Edit2, Trash2, ArrowDown, Loader } from 'lucide-vue-next'
import ConfirmDialog from '../components/ConfirmDialog.vue'
import { useToast } from '../composables/useToast'

const toast = useToast()
const list = ref([])
const searchQuery = ref('')
const showForm = ref(false)
const isLoading = ref(false)
const editingCompany = ref(null)
const showDeleteConfirm = ref(false)
const companyToDelete = ref(null)

const currentCompany = ref({
    companyName: '',
    taxCode: '',
    address: '',
    phoneNumber: ''
})

const filteredList = computed(() => {
    if (!searchQuery.value) return list.value
    const query = searchQuery.value.toLowerCase()
    return list.value.filter(c => 
        c.companyName.toLowerCase().includes(query) ||
        c.taxCode.toLowerCase().includes(query) ||
        c.address.toLowerCase().includes(query) ||
        c.phoneNumber.includes(query)
    )
})

const fetchList = async () => {
    try {
        const res = await axios.get('http://localhost:5283/api/Companies')
        list.value = res.data
    } catch (e) { 
        console.error(e)
        toast.error('Không thể tải danh sách công ty')
    }
}

const addCompany = async () => {
    try {
        const { companyName, taxCode, address, phoneNumber } = currentCompany.value;
        
        if (!companyName) return toast.warning('Bạn chưa nhập tên công ty!')
        if (!taxCode) return toast.warning('Bạn chưa nhập mã số thuế!')
        if (!address) return toast.warning('Bạn chưa nhập địa chỉ!')
        if (!phoneNumber || phoneNumber.length < 10) return toast.warning('Số điện thoại phải đủ 10 chữ số!')

        isLoading.value = true
        await axios.post('http://localhost:5283/api/Companies', currentCompany.value)
        toast.success('Đã thêm đối tác mới thành công!')
        await fetchList()
        showForm.value = false
        resetForm()
    } catch (e) { 
        console.error(e) 
        toast.error('Không thể lưu thông tin công ty')
    } finally {
        isLoading.value = false
    }
}

const editCompany = (company) => {
    editingCompany.value = company
    currentCompany.value = { ...company }
    showForm.value = false
}

const updateCompany = async () => {
    try {
        isLoading.value = true
        await axios.put(`http://localhost:5283/api/Companies/${editingCompany.value.companyId}`, currentCompany.value)
        toast.success('Cập nhật thông tin thành công!')
        await fetchList()
        cancelEdit()
    } catch (e) {
        console.error(e)
        toast.error('Không thể cập nhật thông tin công ty')
    } finally {
        isLoading.value = false
    }
}

const confirmDelete = (company) => {
    companyToDelete.value = company
    showDeleteConfirm.value = true
}

const deleteCompany = async () => {
    try {
        isLoading.value = true
        await axios.delete(`http://localhost:5283/api/Companies/${companyToDelete.value.companyId}`)
        toast.success(`Đã xóa "${companyToDelete.value.companyName}"`)
        await fetchList()
        companyToDelete.value = null
    } catch (e) {
        console.error(e)
        toast.error('Không thể xóa công ty này')
    } finally {
        isLoading.value = false
    }
}

const cancelEdit = () => {
    editingCompany.value = null
    showForm.value = false
    resetForm()
}

const resetForm = () => {
    currentCompany.value = { companyName: '', taxCode: '', address: '', phoneNumber: '' }
}

const handlePhoneInput = (e) => {
    let value = e.target.value.replace(/\D/g, '')
    currentCompany.value.phoneNumber = value.slice(0, 10)
    
    let formatted = currentCompany.value.phoneNumber
    if (formatted.length > 4) formatted = formatted.slice(0, 4) + '.' + formatted.slice(4)
    if (formatted.length > 8) formatted = formatted.slice(0, 8) + '.' + formatted.slice(8)
    
    e.target.value = formatted
}

const formatPhone = (val) => {
    if (!val) return ''
    let cleaned = val.replace(/\D/g, '')
    return cleaned.replace(/(\d{4})(\d{3})(\d{3})/, '$1.$2.$3')
}

onMounted(fetchList)
</script>
