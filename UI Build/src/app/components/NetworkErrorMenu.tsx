import { motion } from 'motion/react';
import { WifiOff, RotateCw, Cloud } from 'lucide-react';

interface NetworkErrorMenuProps {
  onOffline: () => void;
  onRetry: () => void;
}

export function NetworkErrorMenu({ onOffline, onRetry }: NetworkErrorMenuProps) {
  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.8 }}
      animate={{ opacity: 1, scale: 1 }}
      className="fixed inset-0 flex items-center justify-center bg-black/50 z-50"
    >
      <motion.div
        initial={{ y: -50 }}
        animate={{ y: 0 }}
        transition={{ type: "spring", bounce: 0.5 }}
        className="relative bg-gradient-to-b from-gray-600 to-gray-800 rounded-3xl p-8 shadow-2xl max-w-md w-full mx-4"
      >
        {/* Декоративные облака */}
        <motion.div
          animate={{ 
            x: [0, 10, 0],
            opacity: [0.3, 0.5, 0.3]
          }}
          transition={{ 
            duration: 4,
            repeat: Infinity,
            ease: "easeInOut"
          }}
          className="absolute top-4 right-8 text-white/20"
        >
          <Cloud size={40} />
        </motion.div>
        <motion.div
          animate={{ 
            x: [0, -10, 0],
            opacity: [0.2, 0.4, 0.2]
          }}
          transition={{ 
            duration: 5,
            repeat: Infinity,
            ease: "easeInOut"
          }}
          className="absolute bottom-12 left-6 text-white/20"
        >
          <Cloud size={50} />
        </motion.div>

        {/* Иконка WiFi */}
        <motion.div
          animate={{ 
            scale: [1, 1.1, 1],
          }}
          transition={{ 
            duration: 2,
            repeat: Infinity,
          }}
          className="flex justify-center mb-6 relative z-10"
        >
          <div className="bg-white rounded-full p-6 shadow-lg">
            <WifiOff size={64} className="text-gray-600" />
          </div>
        </motion.div>

        {/* Заголовок */}
        <motion.h1
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          transition={{ delay: 0.2, type: "spring", bounce: 0.6 }}
          className="text-4xl font-bold text-white text-center mb-2 drop-shadow-lg relative z-10"
        >
          Ошибка сети
        </motion.h1>

        <motion.p
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.3 }}
          className="text-white/80 text-center mb-8 relative z-10"
        >
          Проверьте подключение к интернету
        </motion.p>

        {/* Кнопки */}
        <div className="space-y-4 relative z-10">
          <motion.button
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.4 }}
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            onClick={onRetry}
            className="w-full bg-blue-500 hover:bg-blue-600 text-white font-bold py-4 px-8 rounded-2xl shadow-lg hover:shadow-xl transition-all text-xl flex items-center justify-center gap-3"
          >
            <motion.div
              animate={{ rotate: 360 }}
              transition={{ duration: 1, repeat: Infinity, ease: "linear" }}
            >
              <RotateCw size={24} />
            </motion.div>
            Повторить попытку
          </motion.button>

          <motion.button
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.5 }}
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            onClick={onOffline}
            className="w-full bg-white/20 backdrop-blur-sm hover:bg-white/30 text-white font-bold py-4 px-8 rounded-2xl shadow-lg hover:shadow-xl transition-all text-xl border-2 border-white/30"
          >
            Офлайн режим
          </motion.button>
        </div>
      </motion.div>
    </motion.div>
  );
}
