<template>
  <Transition name="toast">
    <div v-if="visible" 
         :class="['fixed top-6 right-6 z-[9999] px-6 py-4 rounded-2xl shadow-2xl border-2 flex items-center space-x-4 max-w-md animate-slide-in', variantClasses]">
      <component :is="icon" :class="['w-6 h-6', iconColorClass]" />
      <div class="flex-1">
        <p class="font-black text-sm">{{ message }}</p>
      </div>
      <button @click="close" class="hover:opacity-70 transition-opacity">
        <X class="w-5 h-5" />
      </button>
    </div>
  </Transition>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { CheckCircle, XCircle, AlertCircle, Info, X } from 'lucide-vue-next'

const props = defineProps({
  message: { type: String, required: true },
  type: { type: String, default: 'success' }, // success, error, warning, info
  duration: { type: Number, default: 4000 }
})

const emit = defineEmits(['close'])

const visible = ref(false)

const variantClasses = computed(() => {
  switch(props.type) {
    case 'success': return 'bg-success/10 border-success text-success'
    case 'error': return 'bg-rose-50 border-rose-300 text-rose-700'
    case 'warning': return 'bg-amber-50 border-amber-300 text-amber-700'
    case 'info': return 'bg-sky-50 border-sky-300 text-sky-700'
    default: return 'bg-slate-50 border-slate-300 text-slate-700'
  }
})

const iconColorClass = computed(() => {
  switch(props.type) {
    case 'success': return 'text-success'
    case 'error': return 'text-rose-600'
    case 'warning': return 'text-amber-600'
    case 'info': return 'text-sky-600'
    default: return 'text-slate-600'
  }
})

const icon = computed(() => {
  switch(props.type) {
    case 'success': return CheckCircle
    case 'error': return XCircle
    case 'warning': return AlertCircle
    case 'info': return Info
    default: return Info
  }
})

const close = () => {
  visible.value = false
  setTimeout(() => emit('close'), 300)
}

onMounted(() => {
  visible.value = true
  if (props.duration > 0) {
    setTimeout(close, props.duration)
  }
})
</script>

<style scoped>
.toast-enter-active, .toast-leave-active {
  transition: all 0.3s ease;
}
.toast-enter-from {
  opacity: 0;
  transform: translateX(100px);
}
.toast-leave-to {
  opacity: 0;
  transform: translateX(100px);
}

.animate-slide-in {
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(100px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}
</style>
