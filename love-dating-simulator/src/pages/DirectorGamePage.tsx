import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useGame } from '../context/GameContext';

const DirectorGamePage: React.FC = () => {
  const navigate = useNavigate();
  const { gameState } = useGame();

  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <h2>导演视角 - 第{gameState.day}天</h2>
        <div style={styles.stats}>
          <span>💰 {gameState.gold}金币</span>
          <span style={styles.hot}>🔥 节目热度: 85%</span>
        </div>
      </div>
      <div style={styles.content}>
        <p>欢迎来到导演工作室！</p>
        <div style={styles.buttonGroup}>
          <button style={styles.button}>安排活动</button>
          <button style={styles.button}>管理嘉宾配对</button>
          <button style={styles.button}>节目剪辑</button>
        </div>
      </div>
      <button style={styles.backButton} onClick={() => navigate('/')}>
        返回主菜单
      </button>
    </div>
  );
};

const styles = {
  container: {
    minHeight: '100vh',
    backgroundColor: '#F0FFF0',
    padding: '20px'
  },
  header: {
    display: 'flex' as const,
    justifyContent: 'space-between' as const,
    alignItems: 'center' as const,
    marginBottom: '32px'
  },
  stats: {
    fontSize: '18px',
    color: '#333'
  },
  hot: {
    marginLeft: '20px'
  },
  content: {
    textAlign: 'center' as const,
    marginTop: '60px'
  },
  buttonGroup: {
    display: 'flex' as const,
    flexDirection: 'column' as const,
    gap: '16px',
    alignItems: 'center' as const,
    marginTop: '40px'
  },
  button: {
    padding: '16px 32px',
    fontSize: '18px',
    backgroundColor: '#50C878',
    color: 'white',
    border: 'none',
    borderRadius: '8px',
    cursor: 'pointer' as const,
    minWidth: '200px'
  },
  backButton: {
    marginTop: '40px',
    padding: '12px 24px',
    fontSize: '16px',
    backgroundColor: 'transparent',
    color: '#50C878',
    border: '2px solid #50C878',
    borderRadius: '8px',
    cursor: 'pointer' as const
  }
};

export default DirectorGamePage;
