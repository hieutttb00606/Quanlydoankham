<template>
  <div class="min-h-screen flex items-center justify-center bg-[#f0f9ff] p-6 relative overflow-hidden">
    <!-- Decorative geometric shapes -->
    <div class="absolute top-0 right-0 -mr-20 -mt-20 w-80 h-80 bg-primary/10 rounded-full blur-3xl animate-pulse"></div>
    <div class="absolute bottom-0 left-0 -ml-20 -mb-20 w-80 h-80 bg-accent/10 rounded-full blur-3xl animate-pulse" style="animation-delay: 1s"></div>
    
    <div class="w-full max-w-5xl bg-white rounded-[2rem] shadow-2xl overflow-hidden flex relative z-10 border border-slate-100">
      <!-- Left side: Illustration area -->
      <div class="hidden lg:flex w-1/2 bg-gradient-to-br from-primary to-primary-dark p-12 flex-col justify-between text-white relative">
        <div class="flex items-center space-x-2">
            <div class="bg-white/20 p-2 rounded-lg backdrop-blur-md">
                <Stethoscope class="text-white w-6 h-6" />
            </div>
            <span class="font-extrabold text-2xl tracking-tight">HealthCare</span>
        </div>
        
        <div>
            <h1 class="text-5xl font-black leading-tight mb-6">Nền tảng Y tế <br/> số một Việt Nam</h1>
            <p class="text-primary-light text-lg font-medium max-w-md">
                Cung cấp giải pháp tối ưu cho việc quản lý đoàn khám sức khỏe doanh nghiệp, hỗ trợ bởi trí tuệ nhân tạo.
            </p>
        </div>
        
        <div class="flex items-center space-x-6">
            <div class="flex -space-x-3">
                <div v-for="i in 4" :key="i" class="w-10 h-10 rounded-full border-2 border-primary bg-slate-200 overflow-hidden">
                    <img :src="`https://api.dicebear.com/7.x/avataaars/svg?seed=${i}`" alt="user" />
                </div>
            </div>
            <span class="text-sm font-bold">+5,000 khách hàng tin dùng</span>
        </div>

        <!-- Floatting element -->
        <div class="absolute top-1/2 right-0 translate-x-1/2 -translate-y-1/2 bg-white/10 backdrop-blur-xl p-6 rounded-3xl border border-white/20 shadow-2xl">
            <Sparkles class="text-accent w-8 h-8 mb-4" />
            <div class="space-y-2">
                <div class="h-2 w-24 bg-white/30 rounded"></div>
                <div class="h-2 w-16 bg-white/20 rounded"></div>
            </div>
        </div>
      </div>

      <!-- Right side: Login form -->
      <div class="w-full lg:w-1/2 p-12 md:p-20 bg-white flex flex-col justify-center">
        <div class="mb-12 text-center lg:text-left">
          <div class="lg:hidden flex items-center justify-center space-x-2 mb-8 uppercase tracking-widest text-primary font-black">
             <Stethoscope class="w-6 h-6" />
             <span>HealthCare</span>
          </div>
          <h2 class="text-4xl font-black text-slate-900 mb-2">Chào mừng trở lại!</h2>
          <p class="text-slate-500 font-medium">Vui lòng đăng nhập để quản lý đoàn khám.</p>
        </div>

        <form @submit.prevent="handleLogin" class="space-y-6">
          <div class="space-y-2">
            <label class="text-xs font-black uppercase tracking-widest text-slate-400 ml-1">Tài khoản</label>
            <div class="relative group">
              <span class="absolute inset-y-0 left-0 pl-4 flex items-center text-slate-400 group-focus-within:text-primary transition-colors">
                <User class="w-5 h-5" />
              </span>
              <input 
                v-model="username"
                type="text" 
                class="w-full pl-12 pr-4 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white focus:outline-none transition-all font-bold text-slate-800"
                placeholder="Ví dụ: admin"
                required
              />
            </div>
          </div>

          <div class="space-y-2">
            <div class="flex justify-between items-center px-1">
                <label class="text-xs font-black uppercase tracking-widest text-slate-400">Mật khẩu</label>
                <a href="#" class="text-xs font-bold text-primary hover:underline">Quên mật khẩu?</a>
            </div>
            <div class="relative group">
              <span class="absolute inset-y-0 left-0 pl-4 flex items-center text-slate-400 group-focus-within:text-primary transition-colors">
                <Lock class="w-5 h-5" />
              </span>
              <input 
                v-model="password"
                :type="showPassword ? 'text' : 'password'" 
                class="w-full pl-12 pr-12 py-4 rounded-2xl bg-slate-50 border-2 border-transparent focus:border-primary/20 focus:bg-white focus:outline-none transition-all font-bold text-slate-800"
                placeholder="••••••••"
                required
              />
              <button 
                type="button"
                @click="showPassword = !showPassword"
                class="absolute inset-y-0 right-0 pr-4 flex items-center text-slate-400 hover:text-primary transition-colors"
              >
                <Eye v-if="!showPassword" class="w-5 h-5" />
                <EyeOff v-else class="w-5 h-5" />
              </button>
            </div>
          </div>

          <transition name="fade">
            <div v-if="authStore.error" class="bg-rose-50 text-rose-600 p-4 rounded-2xl text-sm font-bold flex items-center space-x-3 border border-rose-100">
                <AlertCircle class="w-5 h-5 flex-shrink-0" />
                <span>{{ authStore.error }}</span>
            </div>
          </transition>

          <button 
            :disabled="authStore.loading"
            type="submit" 
            class="w-full bg-primary hover:bg-primary-dark text-white font-black py-4 rounded-2xl shadow-xl shadow-primary/20 hover:shadow-primary/30 transition-all flex items-center justify-center space-x-3 active:scale-[0.98] disabled:opacity-50"
          >
            <span v-if="!authStore.loading">ĐĂNG NHẬP NGAY</span>
            <span v-else class="flex items-center space-x-2">
                <Loader2 class="w-5 h-5 animate-spin" />
                <span>Đang kiểm tra...</span>
            </span>
            <ArrowRight v-if="!authStore.loading" class="w-5 h-5" />
          </button>
        </form>

        <div class="mt-12 text-center">
            <p class="text-slate-500 font-medium">Bạn chưa có tài khoản? <a href="#" class="text-primary font-black hover:underline">Liên hệ hợp tác</a></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { 
  Stethoscope, 
  User, 
  Lock, 
  Eye,
  EyeOff,
  ArrowRight, 
  Loader2, 
  AlertCircle,
  Sparkles
} from 'lucide-vue-next'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const password = ref('')
const showPassword = ref(false)

const handleLogin = async () => {
  const success = await authStore.login(username.value, password.value)
  if (success) {
    router.push('/')
  }
}
</script>

<style scoped>
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
</style>
