<template>
  <div class="p-8 bg-gradient-to-br from-slate-50 to-blue-50 min-h-screen">
    <!-- Header -->
    <div class="mb-8">
      <h1 class="text-4xl font-black text-slate-900 mb-2">Quản lý Bệnh nhân</h1>
      <p class="text-slate-600 font-medium">Quản lý danh sách bệnh nhân và kết quả khám sức khỏe</p>
    </div>

    <!-- Contract Selection -->
    <div class="bg-white rounded-2xl shadow-lg p-6 mb-6">
      <label class="block text-sm font-bold text-slate-700 mb-2">Chọn Hợp đồng</label>
      <select 
        v-model="selectedContractId" 
        @change="fetchPatients"
        class="w-full md:w-96 px-4 py-3 rounded-xl border-2 border-slate-200 focus:border-primary focus:outline-none font-medium"
      >
        <option :value="null">-- Chọn hợp đồng --</option>
        <option v-for="contract in contracts" :key="contract.contractId" :value="contract.contractId">
          {{ contract.companyName }} - {{ contract.contractName }}
        </option>
      </select>
    </div>

    <!-- Patients List -->
    <div v-if="selectedContractId" class="bg-white rounded-2xl shadow-lg p-6">
      <div class="flex justify-between items-center mb-6">
        <h2 class="text-2xl font-black text-slate-900">Danh sách Bệnh nhân</h2>
        <button 
          @click="openModal()"
          class="flex items-center space-x-2 bg-primary hover:bg-primary-dark text-white px-6 py-3 rounded-xl font-bold shadow-lg transition-all"
        >
          <UserPlus class="w-5 h-5" />
          <span>Thêm Bệnh nhân</span>
        </button>
      </div>

      <!-- Patients Table -->
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead>
            <tr class="border-b-2 border-slate-200">
              <th class="text-left py-4 px-4 font-black text-slate-700">Họ tên</th>
              <th class="text-left py-4 px-4 font-black text-slate-700">Ngày sinh</th>
              <th class="text-left py-4 px-4 font-black text-slate-700">Giới tính</th>
              <th class="text-left py-4 px-4 font-black text-slate-700">CMND</th>
              <th class="text-left py-4 px-4 font-black text-slate-700">Phòng ban</th>
              <th class="text-right py-4 px-4 font-black text-slate-700">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="patient in patients" :key="patient.patientId" class="border-b border-slate-100 hover:bg-slate-50">
              <td class="py-4 px-4 font-bold text-slate-900">{{ patient.fullName }}</td>
              <td class="py-4 px-4 text-slate-600">{{ formatDate(patient.dateOfBirth) }}</td>
              <td class="py-4 px-4 text-slate-600">{{ patient.gender }}</td>
              <td class="py-4 px-4 text-slate-600">{{ patient.idCardNumber }}</td>
              <td class="py-4 px-4 text-slate-600">{{ patient.department }}</td>
              <td class="py-4 px-4 text-right">
                <button 
                  @click="viewExamResults(patient)"
                  class="text-primary hover:text-primary-dark font-bold mr-3"
                >
                  <FileText class="w-5 h-5 inline" />
                </button>
                <button 
                  @click="deletePatient(patient.patientId)"
                  class="text-rose-600 hover:text-rose-700 font-bold"
                >
                  <Trash2 class="w-5 h-5 inline" />
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add Patient Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div class="bg-white rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div class="p-6 border-b border-slate-200">
          <h3 class="text-2xl font-black text-slate-900">Thêm Bệnh nhân</h3>
        </div>
        
        <div class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-bold text-slate-700 mb-2">Họ tên *</label>
            <input v-model="currentPatient.fullName" type="text" class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 focus:border-primary focus:outline-none" required />
          </div>
          
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Ngày sinh *</label>
              <input v-model="currentPatient.dateOfBirth" type="date" class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 focus:border-primary focus:outline-none" required />
            </div>
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Giới tính *</label>
              <select v-model="currentPatient.gender" class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 focus:border-primary focus:outline-none">
                <option value="Nam">Nam</option>
                <option value="Nữ">Nữ</option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">CMND/CCCD</label>
              <input v-model="currentPatient.idCardNumber" type="text" class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 focus:border-primary focus:outline-none" />
            </div>
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Số điện thoại</label>
              <input v-model="currentPatient.phoneNumber" type="text" class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 focus:border-primary focus:outline-none" />
            </div>
          </div>

          <div>
            <label class="block text-sm font-bold text-slate-700 mb-2">Phòng ban</label>
            <input v-model="currentPatient.department" type="text" class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 focus:border-primary focus:outline-none" />
          </div>
        </div>

        <div class="p-6 border-t border-slate-200 flex justify-end space-x-3">
          <button @click="showModal = false" class="px-6 py-3 rounded-xl font-bold text-slate-700 hover:bg-slate-100">Hủy</button>
          <button @click="savePatient" class="px-6 py-3 rounded-xl font-bold bg-primary hover:bg-primary-dark text-white">Lưu</button>
        </div>
      </div>
    </div>

    <!-- Exam Results Modal -->
    <div v-if="showExamModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div class="bg-white rounded-2xl shadow-2xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
        <div class="p-6 border-b border-slate-200">
          <h3 class="text-2xl font-black text-slate-900">Kết quả khám: {{ selectedPatient?.fullName }}</h3>
        </div>
        
        <div class="p-6">
          <button 
            @click="showAddExamForm = !showAddExamForm"
            class="mb-4 flex items-center space-x-2 bg-primary hover:bg-primary-dark text-white px-4 py-2 rounded-xl font-bold"
          >
            <Plus class="w-5 h-5" />
            <span>Thêm kết quả khám</span>
          </button>

          <!-- Add Exam Form -->
          <div v-if="showAddExamForm" class="bg-slate-50 rounded-xl p-4 mb-6 space-y-3">
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Loại khám</label>
              <input v-model="newExam.examType" type="text" class="w-full px-4 py-2 rounded-xl border-2 border-slate-200 focus:border-primary focus:outline-none" placeholder="Ví dụ: Nội khoa" />
            </div>
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Kết quả</label>
              <textarea v-model="newExam.result" class="w-full px-4 py-2 rounded-xl border-2 border-slate-200 focus:border-primary focus:outline-none" rows="3"></textarea>
            </div>
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Chẩn đoán</label>
              <input v-model="newExam.diagnosis" type="text" class="w-full px-4 py-2 rounded-xl border-2 border-slate-200 focus:border-primary focus:outline-none" />
            </div>
            <button @click="saveExamResult" class="bg-primary hover:bg-primary-dark text-white px-6 py-2 rounded-xl font-bold">Lưu kết quả</button>
          </div>

          <!-- Exam Results List -->
          <div class="space-y-3">
            <div v-for="exam in examResults" :key="exam.examResultId" class="border-2 border-slate-200 rounded-xl p-4">
              <div class="flex justify-between items-start mb-2">
                <h4 class="font-black text-slate-900">{{ exam.examType }}</h4>
                <span class="text-sm text-slate-500">{{ formatDate(exam.examDate) }}</span>
              </div>
              <p class="text-slate-700 mb-2"><strong>Kết quả:</strong> {{ exam.result }}</p>
              <p class="text-slate-700"><strong>Chẩn đoán:</strong> {{ exam.diagnosis }}</p>
              <p v-if="exam.doctorName" class="text-sm text-slate-500 mt-2">Bác sĩ: {{ exam.doctorName }}</p>
            </div>
          </div>
        </div>

        <div class="p-6 border-t border-slate-200 flex justify-end">
          <button @click="closeExamModal" class="px-6 py-3 rounded-xl font-bold text-slate-700 hover:bg-slate-100">Đóng</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { UserPlus, FileText, Trash2, Plus } from 'lucide-vue-next'

const contracts = ref([])
const selectedContractId = ref(null)
const patients = ref([])
const showModal = ref(false)
const showExamModal = ref(false)
const showAddExamForm = ref(false)
const selectedPatient = ref(null)
const examResults = ref([])

const currentPatient = ref({
  fullName: '',
  dateOfBirth: '',
  gender: 'Nam',
  idCardNumber: '',
  phoneNumber: '',
  department: ''
})

const newExam = ref({
  examType: '',
  result: '',
  diagnosis: ''
})

onMounted(async () => {
  await fetchContracts()
})

const fetchContracts = async () => {
  try {
    const res = await axios.get('http://localhost:5283/api/HealthContracts')
    contracts.value = res.data
  } catch (e) {
    console.error(e)
  }
}

const fetchPatients = async () => {
  if (!selectedContractId.value) return
  try {
    const res = await axios.get(`http://localhost:5283/api/Patients/by-contract/${selectedContractId.value}`)
    patients.value = res.data
  } catch (e) {
    console.error(e)
  }
}

const openModal = () => {
  currentPatient.value = {
    fullName: '',
    dateOfBirth: '',
    gender: 'Nam',
    idCardNumber: '',
    phoneNumber: '',
    department: ''
  }
  showModal.value = true
}

const savePatient = async () => {
  try {
    await axios.post('http://localhost:5283/api/Patients', {
      ...currentPatient.value,
      healthContractId: selectedContractId.value
    })
    alert('✅ Đã thêm bệnh nhân!')
    showModal.value = false
    fetchPatients()
  } catch (e) {
    console.error(e)
    alert('❌ Lỗi khi thêm bệnh nhân')
  }
}

const viewExamResults = async (patient) => {
  selectedPatient.value = patient
  showExamModal.value = true
  showAddExamForm.value = false
  try {
    const res = await axios.get(`http://localhost:5283/api/Patients/${patient.patientId}/exam-results`)
    examResults.value = res.data
  } catch (e) {
    console.error(e)
  }
}

const saveExamResult = async () => {
  try {
    await axios.post(`http://localhost:5283/api/Patients/${selectedPatient.value.patientId}/exam-result`, newExam.value)
    alert('✅ Đã lưu kết quả khám!')
    showAddExamForm.value = false
    newExam.value = { examType: '', result: '', diagnosis: '' }
    viewExamResults(selectedPatient.value)
  } catch (e) {
    console.error(e)
    alert('❌ Lỗi khi lưu kết quả')
  }
}

const closeExamModal = () => {
  showExamModal.value = false
  selectedPatient.value = null
  examResults.value = []
}

const deletePatient = async (id) => {
  if (!confirm('Bạn có chắc muốn xóa bệnh nhân này?')) return
  try {
    await axios.delete(`http://localhost:5283/api/Patients/${id}`)
    alert('✅ Đã xóa bệnh nhân!')
    fetchPatients()
  } catch (e) {
    console.error(e)
    alert('❌ Lỗi khi xóa')
  }
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('vi-VN')
}
</script>
