import { motion } from 'motion/react';
import { Trophy, Sparkles, Coins } from 'lucide-react';

interface VictoryMenuProps {
  coins: number;
  onPlayAgain: () => void;
}

export function VictoryMenu({ coins, onPlayAgain }: VictoryMenuProps) {
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
        className="relative bg-gradient-to-b from-yellow-400 to-orange-500 rounded-3xl p-8 shadow-2xl max-w-md w-full mx-4"
      >
        {/* Декоративные элементы */}
        <motion.div
          animate={{ rotate: 360 }}
          transition={{ duration: 20, repeat: Infinity, ease: "linear" }}
          className="absolute -top-6 -right-6 text-yellow-300 opacity-50"
        >
          <Sparkles size={60} />
        </motion.div>
        <motion.div
          animate={{ rotate: -360 }}
          transition={{ duration: 15, repeat: Infinity, ease: "linear" }}
          className="absolute -bottom-4 -left-4 text-yellow-300 opacity-50"
        >
          <Sparkles size={50} />
        </motion.div>

        {/* Иконка трофея */}
        <motion.div
          animate={{ 
            scale: [1, 1.1, 1],
            rotate: [0, -5, 5, -5, 0]
          }}
          transition={{ 
            duration: 2,
            repeat: Infinity,
            repeatDelay: 1
          }}
          className="flex justify-center mb-6"
        >
          <div className="bg-white rounded-full p-6 shadow-lg">
            <Trophy size={64} className="text-yellow-500" />
          </div>
        </motion.div>

        {/* Заголовок */}
        <motion.h1
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          transition={{ delay: 0.2, type: "spring", bounce: 0.6 }}
          className="text-5xl font-bold text-white text-center mb-4 drop-shadow-lg"
        >
          ПОБЕДА!
        </motion.h1>

        {/* Монеты */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.4 }}
          className="bg-white/20 backdrop-blur-sm rounded-2xl p-6 mb-6"
        >
          <div className="flex items-center justify-center gap-3">
            <motion.div
              animate={{ rotate: [0, 360] }}
              transition={{ duration: 2, repeat: Infinity, ease: "linear" }}
            >
              <Coins size={40} className="text-yellow-200" />
            </motion.div>
            <div className="text-center">
              <p className="text-white/80 text-sm font-medium">Заработано</p>
              <motion.p
                initial={{ scale: 0 }}
                animate={{ scale: 1 }}
                transition={{ delay: 0.6, type: "spring", bounce: 0.7 }}
                className="text-4xl font-bold text-white drop-shadow-lg"
              >
                +{coins}
              </motion.p>
            </div>
          </div>
        </motion.div>

        {/* Кнопка */}
        <motion.button
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.6 }}
          whileHover={{ scale: 1.05 }}
          whileTap={{ scale: 0.95 }}
          onClick={onPlayAgain}
          className="w-full bg-white text-orange-600 font-bold py-4 px-8 rounded-2xl shadow-lg hover:shadow-xl transition-shadow text-xl"
        >
          Играть снова
        </motion.button>
      </motion.div>
    </motion.div>
  );
}
