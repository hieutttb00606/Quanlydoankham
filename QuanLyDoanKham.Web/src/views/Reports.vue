<template>
  <div class="space-y-10 animate-fade-in pb-20">
    <div class="header">
      <h2 class="text-4xl font-black text-slate-800 tracking-tighter uppercase mb-2">Thống kê & Báo cáo</h2>
      <p class="text-slate-400 font-bold">Dữ liệu kinh doanh thời gian thực từ hệ thống</p>
    </div>

    <!-- Dashboard Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
        <div v-for="stat in statCards" :key="stat.label" class="bg-white p-10 rounded-[3rem] border border-slate-100 shadow-xl relative overflow-hidden group">
            <div :class="['absolute right-0 top-0 w-24 h-24 rounded-bl-[4rem] flex items-center justify-center opacity-10 group-hover:scale-110 transition-transform', stat.colorClass]">
                <component :is="stat.icon" class="w-12 h-12" />
            </div>
            <p class="text-xs font-black text-slate-400 uppercase tracking-widest mb-4">{{ stat.label }}</p>
            <p class="text-4xl font-black text-slate-900 leading-none">{{ stat.value || 0 }}</p>
        </div>
    </div>

    <!-- Revenue vs Profit Area -->
    <div class="bg-slate-900 rounded-[4rem] p-12 text-white overflow-hidden relative shadow-2xl">
        <div class="absolute top-0 right-0 p-12 opacity-5 scale-150">
            <BarChart3 class="w-64 h-64" />
        </div>
        
        <div class="relative z-10 grid grid-cols-1 lg:grid-cols-2 gap-20">
            <div class="space-y-8">
                <h3 class="text-3xl font-black tracking-tight text-white/90">Kinh doanh tổng quát</h3>
                <p class="text-slate-400 font-bold leading-relaxed max-w-md">Phân tích chênh lệch giữa doanh thu từ hợp đồng và các chi phí vận hành (Lương nhân viên + Vật tư y tế).</p>
                <div class="flex space-x-4">
                    <div class="bg-primary/20 p-4 rounded-3xl border border-primary/20">
                        <TrendingUp class="w-8 h-8 text-primary" />
                    </div>
                </div>
            </div>
            
            <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div class="bg-white/5 border border-white/10 p-8 rounded-[2.5rem] backdrop-blur-md">
                    <p class="text-sky-400 text-[10px] font-black uppercase tracking-widest mb-2">Tổng doanh thu</p>
                    <p class="text-3xl font-black">{{ formatPrice(stats.totalRevenue) }}</p>
                </div>
                <div class="bg-white/5 border border-white/10 p-8 rounded-[2.5rem] backdrop-blur-md">
                    <p class="text-rose-400 text-[10px] font-black uppercase tracking-widest mb-2">Chi phí vận hành</p>
                    <p class="text-3xl font-black">{{ formatPrice(stats.totalSalaryCost + stats.totalSupplyCost) }}</p>
                </div>
                <div class="bg-success/10 border border-success/20 p-8 rounded-[2.5rem] col-span-1 md:col-span-2">
                    <p class="text-success text-[10px] font-black uppercase tracking-widest mb-2">Lợi nhuận ròng (Net Profit)</p>
                    <p class="text-5xl font-black text-success">{{ formatPrice(stats.netProfit) }}</p>
                </div>
            </div>
        </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Top Supplies Table -->
        <div class="lg:col-span-1 bg-white p-10 rounded-[3rem] border border-slate-100 shadow-xl">
            <h3 class="text-xl font-black mb-8 flex items-center">
                <Package class="w-6 h-6 mr-3 text-indigo-500" />
                Vật tư tiêu thụ nhiều
            </h3>
            <div class="space-y-6">
                <div v-for="s in stats.topSupplies" :key="s.name" class="flex items-center justify-between group">
                    <div>
                        <p class="font-black text-slate-800 group-hover:text-primary transition-colors">{{ s.name }}</p>
                        <p class="text-xs font-bold text-slate-400 uppercase tracking-widest">Đã xuất: {{ s.qty }}</p>
                    </div>
                    <div class="text-right">
                        <p class="font-black text-slate-700">{{ formatPrice(s.cost) }}</p>
                    </div>
                </div>
                <div v-if="!stats.topSupplies || stats.topSupplies.length === 0" class="text-center py-10 text-slate-300 italic font-bold">Chưa có dữ liệu</div>
            </div>
        </div>

        <!-- Group Performance Table -->
        <div class="lg:col-span-2 bg-white p-10 rounded-[3rem] border border-slate-100 shadow-xl">
            <h3 class="text-xl font-black mb-8 flex items-center">
                <Stethoscope class="w-6 h-6 mr-3 text-amber-500" />
                Vận hành Đoàn khám
            </h3>
            <div class="overflow-x-auto">
                <table class="w-full">
                    <thead>
                        <tr class="text-left border-b border-slate-50">
                            <th class="pb-4 text-[10px] font-black uppercase tracking-widest text-slate-400">Tên đoàn / Công ty</th>
                            <th class="pb-4 text-[10px] font-black uppercase tracking-widest text-slate-400">Nhân sự</th>
                            <th class="pb-4 text-[10px] font-black uppercase tracking-widest text-slate-400">Tổng chi phí</th>
                        </tr>
                    </thead>
                    <tbody class="divide-y divide-slate-50">
                        <tr v-for="g in stats.groupStats" :key="g.groupName" class="group hover:bg-slate-50/50 transition-all">
                            <td class="py-5">
                                <p class="font-black text-slate-800">{{ g.groupName }}</p>
                                <p class="text-xs font-bold text-slate-400">{{ g.companyName }}</p>
                            </td>
                            <td class="py-5">
                                <div class="flex items-center space-x-2">
                                    <Users class="w-4 h-4 text-slate-400" />
                                    <span class="font-black text-slate-700">{{ g.staffCount }}</span>
                                </div>
                            </td>
                            <td class="py-5 text-right">
                                <p class="font-black text-rose-500">{{ formatPrice(g.totalCost) }}</p>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div v-if="!stats.groupStats || stats.groupStats.length === 0" class="text-center py-20 text-slate-300 italic font-bold">Chưa có dữ liệu vận hành</div>
            </div>
        </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { FileText, Stethoscope, Users, Package, BarChart3, TrendingUp } from 'lucide-vue-next'

const stats = ref({
    totalContracts: 0,
    totalRevenue: 0,
    totalSalaryCost: 0,
    totalSupplyCost: 0,
    netProfit: 0,
    activeGroups: 0,
    staffParticipating: 0,
    suppliesExported: 0,
    topSupplies: [],
    groupStats: []
})

const fetchStats = async () => {
    try {
        const res = await axios.get('http://localhost:5283/api/Reports/dashboard')
        // Defensive mapping to handle both camelCase and PascalCase
        const d = res.data
        stats.value = {
            totalContracts: d.totalContracts ?? d.TotalContracts ?? 0,
            totalRevenue: d.totalRevenue ?? d.TotalRevenue ?? 0,
            totalSalaryCost: d.totalSalaryCost ?? d.TotalSalaryCost ?? 0,
            totalSupplyCost: d.totalSupplyCost ?? d.TotalSupplyCost ?? 0,
            netProfit: d.netProfit ?? d.NetProfit ?? 0,
            activeGroups: d.activeGroups ?? d.ActiveGroups ?? 0,
            staffParticipating: d.staffParticipating ?? d.StaffParticipating ?? 0,
            suppliesExported: d.suppliesExported ?? d.SuppliesExported ?? 0,
            topSupplies: d.topSupplies ?? d.TopSupplies ?? [],
            groupStats: d.groupStats ?? d.GroupStats ?? []
        }
    } catch (e) {
        console.error("Failed to fetch report stats", e)
    }
}

const statCards = computed(() => [
    { label: 'Tổng Hợp đồng', value: stats.value.totalContracts, icon: FileText, colorClass: 'bg-sky-500' },
    { label: 'Đoàn đang vận hành', value: stats.value.activeGroups, icon: Stethoscope, colorClass: 'bg-amber-500' },
    { label: 'Nhân sự tham gia', value: stats.value.staffParticipating, icon: Users, colorClass: 'bg-rose-500' },
    { label: 'Vật phẩm đã xuất', value: stats.value.suppliesExported, icon: Package, colorClass: 'bg-indigo-500' }
])

const formatPrice = (p) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p || 0)

onMounted(fetchStats)
</script>
