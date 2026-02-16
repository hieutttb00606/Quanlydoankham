<template>
  <div class="space-y-6 animate-fade-in">
    <div class="flex justify-between items-center text-slate-800">
      <h2 class="text-3xl font-black">Kho Vật tư Y tế</h2>
      <button @click="showForm = !showForm" :disabled="isLoading"
              class="bg-amber-500 hover:bg-amber-600 text-white px-6 py-3 rounded-2xl font-bold flex items-center space-x-2 transition-all shadow-lg active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed">
        <Plus class="w-5 h-5" />
        <span>Nhập vật tư mới</span>
      </button>
    </div>

    <!-- Tab Navigation -->
    <div class="flex space-x-3 border-b-2 border-slate-100">
      <button @click="activeTab = 'inventory'" :class="['px-6 py-3 font-black text-sm transition-all relative', activeTab === 'inventory' ? 'text-amber-600' : 'text-slate-400 hover:text-slate-600']">
        <Package class="w-4 h-4 inline mr-2" />
        Tồn kho
        <div v-if="activeTab === 'inventory'" class="absolute bottom-0 left-0 right-0 h-1 bg-amber-500 rounded-t-full"></div>
      </button>
      <button @click="activeTab = 'history'" :class="['px-6 py-3 font-black text-sm transition-all relative', activeTab === 'history' ? 'text-amber-600' : 'text-slate-400 hover:text-slate-600']">
        <Clock class="w-4 h-4 inline mr-2" />
        Lịch sử giao dịch
        <div v-if="activeTab === 'history'" class="absolute bottom-0 left-0 right-0 h-1 bg-amber-500 rounded-t-full"></div>
      </button>
    </div>

    <!-- Inventory Tab -->
    <div v-show="activeTab === 'inventory'">
      <!-- Form -->
      <div v-if="showForm || editingSupply" class="bg-white p-8 rounded-[2rem] border border-amber-100 shadow-xl mb-10">
          <h3 class="text-xl font-black mb-6 text-slate-800 flex items-center">
              <div class="w-2 h-6 bg-amber-400 rounded-full mr-3"></div>
              {{ editingSupply ? 'Chỉnh sửa vật tư' : 'Khai báo vật tư/thiết bị' }}
          </h3>
          <form @submit.prevent="editingSupply ? updateSupply() : addSupply()" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              <div class="space-y-2 lg:col-span-2">
                  <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Tên vật tư</label>
                  <input v-model="currentSupply.supplyName" required :disabled="isLoading" class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-amber-400/20 focus:bg-white outline-none transition-all font-bold disabled:opacity-50" />
              </div>
              <div class="space-y-2">
                  <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Loại tài sản</label>
                  <select v-model="currentSupply.isFixedAsset" :disabled="isLoading" class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-amber-400/20 focus:bg-white outline-none transition-all font-bold disabled:opacity-50">
                      <option :value="false">Vật tư tiêu hao</option>
                      <option :value="true">Tài sản cố định (Mượn)</option>
                  </select>
              </div>
              <div class="space-y-2">
                  <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">{{ editingSupply ? 'Tồn kho hiện tại' : 'Số lượng tồn kho' }}</label>
                  <input v-model="currentSupply.stockQuantity" type="number" required :disabled="isLoading" class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-amber-400/20 focus:bg-white outline-none transition-all font-black text-2xl tracking-widest text-slate-700 disabled:opacity-50" />
              </div>
              <div class="space-y-2">
                  <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Đơn giá (VNĐ)</label>
                  <input 
                    type="text" 
                    :value="currentSupply.unitPrice.toLocaleString('vi-VN')" 
                    :disabled="isLoading"
                    @input="e => {
                      const val = e.target.value.replace(/\D/g, '');
                      currentSupply.unitPrice = val ? parseInt(val) : 0;
                      e.target.value = currentSupply.unitPrice.toLocaleString('vi-VN');
                    }"
                    required 
                    class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-amber-400/20 focus:bg-white outline-none transition-all font-black text-2xl tracking-widest text-amber-600 disabled:opacity-50" 
                  />
                  <p v-if="currentSupply.unitPrice > 0" class="text-xs font-black text-amber-600 ml-2 animate-pulse">
                     ➔ {{ formatPrice(currentSupply.unitPrice) }}
                  </p>
              </div>
              <div class="lg:col-span-3 flex items-end justify-end space-x-4">
                  <button type="button" @click="cancelEdit" :disabled="isLoading" class="px-6 py-4 text-slate-400 font-bold hover:bg-slate-50 rounded-xl transition-all disabled:opacity-50">Hủy bỏ</button>
                  <button type="submit" :disabled="isLoading" class="bg-amber-600 text-white px-10 py-4 rounded-2xl font-black shadow-lg hover:bg-amber-700 transition-all disabled:opacity-50 flex items-center space-x-2">
                      <Loader v-if="isLoading" class="w-5 h-5 animate-spin" />
                      <span>{{ isLoading ? 'ĐANG XỬ LÝ...' : (editingSupply ? 'CẬP NHẬT' : 'NHẬP KHO') }}</span>
                  </button>
              </div>
          </form>
      </div>

      <!-- Inventory Grid -->
      <div class="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-6">
          <div v-for="item in list" :key="item.supplyId" class="bg-white p-6 rounded-[2rem] border border-slate-50 shadow-md hover:shadow-xl transition-all group relative">
              <div class="flex justify-between items-start mb-6">
                  <div :class="['p-3 rounded-2xl shadow-inner', item.isFixedAsset ? 'bg-indigo-50 text-indigo-500' : 'bg-amber-50 text-amber-500']">
                      <component :is="item.isFixedAsset ? HardDrive : Droplets" class="w-6 h-6" />
                  </div>
                  <div class="text-right">
                      <span class="text-xs font-bold text-slate-400 uppercase tracking-tighter">{{ item.isFixedAsset ? 'Cố định' : 'Tiêu hao' }}</span>
                      <p class="font-black text-slate-800 text-xl">{{ item.stockQuantity }}</p>
                  </div>
              </div>
              <h4 class="font-black text-slate-800 text-lg mb-4 leading-tight min-h-[50px]">{{ item.supplyName }}</h4>
              <div class="pt-4 border-t border-slate-50 flex justify-between items-center text-sm mb-4">
                  <span class="text-slate-400 font-bold">Đơn giá</span>
                  <span class="font-black text-slate-800">{{ formatPrice(item.unitPrice) }}</span>
              </div>
              <!-- Action Buttons -->
              <div class="flex gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                  <button @click="showRestockModal(item)" class="flex-1 bg-success/10 text-success px-3 py-2 rounded-xl text-xs font-black hover:bg-success/20 transition-all flex items-center justify-center space-x-1">
                      <Package class="w-3 h-3" />
                      <span>NHẬP THÊM</span>
                  </button>
                  <button @click="editSupply(item)" class="bg-indigo-50 text-indigo-600 p-2 rounded-xl hover:bg-indigo-100 transition-all"><Edit2 class="w-4 h-4" /></button>
                  <button @click="confirmDelete(item)" class="bg-rose-50 text-rose-600 p-2 rounded-xl hover:bg-rose-100 transition-all"><Trash2 class="w-4 h-4" /></button>
              </div>
          </div>
          
          <div v-if="list.length === 0" class="col-span-full py-20 text-center text-slate-300 font-bold italic bg-white rounded-[2rem] border-2 border-dashed border-slate-100 uppercase tracking-widest">
              Kho hàng trống
          </div>
      </div>
    </div>

    <!-- History Tab -->
    <div v-show="activeTab === 'history'" class="bg-white rounded-[2rem] shadow-xl overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-slate-800 text-white">
            <tr>
              <th class="px-6 py-4 text-left font-black text-xs uppercase tracking-widest">Ngày</th>
              <th class="px-6 py-4 text-left font-black text-xs uppercase tracking-widest">Loại</th>
              <th class="px-6 py-4 text-left font-black text-xs uppercase tracking-widest">Vật tư</th>
              <th class="px-6 py-4 text-right font-black text-xs uppercase tracking-widest">Số lượng</th>
              <th class="px-6 py-4 text-left font-black text-xs uppercase tracking-widest">Đoàn khám</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="t in supplyHistory" :key="t.transactionId" class="border-b border-slate-100 hover:bg-slate-50 transition-colors group">
              <td class="px-6 py-4 text-sm font-bold text-slate-600">{{ formatDate(t.transactionDate) }}</td>
              <td class="px-6 py-4">
                <span :class="['px-3 py-1 rounded-full text-[10px] font-black uppercase tracking-widest', getHistoryTypeBadge(t.type)]">{{ t.type }}</span>
              </td>
              <td class="px-6 py-4">
                <p class="font-black text-slate-800">{{ t.supplyName }}</p>
                <p class="text-[10px] text-slate-400 font-bold uppercase">{{ t.note }}</p>
              </td>
              <td class="px-6 py-4 text-right">
                <span :class="['font-black text-lg', t.type === 'IMPORT' ? 'text-emerald-500' : 'text-amber-500']">
                  {{ t.type === 'IMPORT' ? '+' : '-' }}{{ t.quantity }}
                </span>
              </td>
              <td class="px-6 py-4 text-sm font-bold text-slate-500 flex items-center space-x-2">
                <div class="w-6 h-6 rounded-full bg-slate-100 flex items-center justify-center text-[10px] font-black uppercase">{{ t.processedBy?.[0] || 'S' }}</div>
                <span>{{ t.processedBy || 'System' }}</span>
              </td>
            </tr>
            <tr v-if="supplyHistory.length === 0">
              <td colspan="5" class="px-6 py-20 text-center text-slate-300 font-bold italic uppercase tracking-widest">
                Chưa có giao dịch nào
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Restock Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showRestock" class="fixed inset-0 z-[9998] flex items-center justify-center bg-slate-900/80 backdrop-blur-sm p-4" @click.self="showRestock = false">
          <div class="bg-white max-w-md w-full p-8 rounded-[2.5rem] shadow-2xl">
            <h3 class="text-xl font-black mb-6 text-slate-800 flex items-center">
              <div class="w-2 h-6 bg-success rounded-full mr-3"></div>
              Nhập thêm hàng
            </h3>
            <p class="text-slate-600 font-bold mb-2">{{ restockItem?.supplyName }}</p>
            <p class="text-sm text-slate-400 mb-6">Tồn kho hiện tại: <span class="font-black text-slate-700">{{ restockItem?.stockQuantity }}</span></p>
            <input v-model="restockQty" type="number" min="1" placeholder="Số lượng nhập thêm" class="w-full px-5 py-4 rounded-2xl bg-slate-50 border-2 border-emerald-500/20 outline-none transition-all font-black text-2xl text-emerald-600 mb-4" />
            <input v-model="restockNote" type="text" placeholder="Lý do nhập (Vd: Nhập mới, Hoàn trả...)" class="w-full px-5 py-4 rounded-xl bg-slate-50 border border-slate-100 outline-none transition-all font-bold text-sm mb-6" />
            <div class="flex space-x-4">
              <button @click="showRestock = false" class="flex-1 px-6 py-3 rounded-xl font-black text-slate-500 hover:bg-slate-100 transition-all">Hủy</button>
              <button @click="restockSupply" class="flex-1 px-6 py-3 rounded-xl font-black text-white bg-success hover:bg-success/90 transition-all">Xác nhận</button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>

    <!-- Confirm Delete Dialog -->
    <ConfirmDialog 
      v-model="showDeleteConfirm"
      title="Xóa vật tư?"
      :message="`Bạn có chắc muốn xóa &quot;${supplyToDelete?.supplyName}&quot;? Dữ liệu không thể khôi phục.`"
      confirmText="Xóa ngay"
      variant="danger"
      @confirm="deleteSupply"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { Plus, ArrowDown, HardDrive, Droplets, Package, Edit2, Trash2, Loader, Clock } from 'lucide-vue-next'
import ConfirmDialog from '../components/ConfirmDialog.vue'
import { useToast } from '../composables/useToast'

const toast = useToast()
const list = ref([])
const activeTab = ref('inventory')
const supplyHistory = ref([])
const showForm = ref(false)
const isLoading = ref(false)
const editingSupply = ref(null)
const showDeleteConfirm = ref(false)
const supplyToDelete = ref(null)
const showRestock = ref(false)
const restockItem = ref(null)
const restockQty = ref(0)
const restockNote = ref('')

const currentSupply = ref({
    supplyName: '',
    isFixedAsset: false,
    unitPrice: 0,
    stockQuantity: 0
})

const fetchList = async () => {
    try {
        const res = await axios.get('http://localhost:5283/api/Supplies')
        list.value = res.data
    } catch (e) { 
        console.error(e)
        toast.error('Không thể tải danh sách vật tư')
    }
}

const fetchHistory = async () => {
    try {
        const res = await axios.get('http://localhost:5283/api/Supplies/transactions')
        supplyHistory.value = res.data
    } catch (e) { 
        console.error(e)
        // toast.error('Không thể tải lịch sử giao dịch')
    }
}

const getHistoryTypeBadge = (type) => {
    if (type === 'IMPORT') return 'bg-emerald-50 text-emerald-600 border border-emerald-100'
    if (type === 'EXPORT') return 'bg-amber-50 text-amber-600 border border-amber-100'
    return 'bg-blue-50 text-blue-600 border border-blue-100'
}

const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('vi-VN', { 
        year: 'numeric', 
        month: '2-digit', 
        day: '2-digit' 
    })
}

const addSupply = async () => {
    try {
        isLoading.value = true
        await axios.post('http://localhost:5283/api/Supplies', currentSupply.value)
        toast.success('Đã nhập vật tư mới thành công!')
        await fetchList()
        showForm.value = false
        resetForm()
    } catch (e) { 
        console.error(e)
        toast.error('Không thể thêm vật tư')
    } finally {
        isLoading.value = false
    }
}

const editSupply = (supply) => {
    editingSupply.value = supply
    currentSupply.value = { ...supply }
    showForm.value = false
}

const updateSupply = async () => {
    try {
        isLoading.value = true
        await axios.put(`http://localhost:5283/api/Supplies/${editingSupply.value.supplyId}`, currentSupply.value)
        toast.success('Cập nhật vật tư thành công!')
        await fetchList()
        cancelEdit()
    } catch (e) {
        console.error(e)
        toast.error('Không thể cập nhật vật tư')
    } finally {
        isLoading.value = false
    }
}

const confirmDelete = (supply) => {
    supplyToDelete.value = supply
    showDeleteConfirm.value = true
}

const deleteSupply = async () => {
    try {
        isLoading.value = true
        await axios.delete(`http://localhost:5283/api/Supplies/${supplyToDelete.value.supplyId}`)
        toast.success(`Đã xóa "${supplyToDelete.value.supplyName}"`)
        await fetchList()
        supplyToDelete.value = null
    } catch (e) {
        console.error(e)
        toast.error('Không thể xóa vật tư này')
    } finally {
        isLoading.value = false
    }
}

const showRestockModal = (item) => {
    restockItem.value = item
    restockQty.value = 0
    showRestock.value = true
}

const restockSupply = async () => {
    if (!restockQty.value || restockQty.value <= 0) {
        return toast.warning('Vui lòng nhập số lượng hợp lệ')
    }
    try {
        await axios.post(`http://localhost:5283/api/Supplies/import/${restockItem.value.supplyId}`, { 
            quantity: restockQty.value,
            note: restockNote.value
        })
        toast.success(`Đã nhập thêm ${restockQty.value} ${restockItem.value.supplyName}`)
        await fetchList()
        await fetchHistory()
        showRestock.value = false
        restockItem.value = null
        restockQty.value = 0
        restockNote.value = ''
    } catch (e) {
        console.error(e)
        toast.error('Không thể nhập thêm hàng')
    }
}

const cancelEdit = () => {
    editingSupply.value = null
    showForm.value = false
    resetForm()
}

const resetForm = () => {
    currentSupply.value = { supplyName: '', isFixedAsset: false, unitPrice: 0, stockQuantity: 0 }
}

const formatPrice = (p) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p)

onMounted(() => {
    fetchList()
    fetchHistory()
})
</script>
