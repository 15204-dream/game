import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useGame } from '../context/GameContext';

const GuestGamePage: React.FC = () => {
  const navigate = useNavigate();
  const { gameState } = useGame();

  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <h2>嘉宾视角 - 第{gameState.day}天</h2>
        <div style={styles.stats}>
          <span>💰 {gameState.gold}金币</span>
        </div>
      </div>
      <div style={styles.content}>
        <p>欢迎来到心动小屋！</p>
        <div style={styles.buttonGroup}>
          <button style={styles.button}>和嘉宾互动</button>
          <button style={styles.button}>查看好感度</button>
          <button style={styles.button}>发送心动短信</button>
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
    backgroundColor: '#E6F3FF',
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
    backgroundColor: '#4A90D9',
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
    color: '#4A90D9',
    border: '2px solid #4A90D9',
    borderRadius: '8px',
    cursor: 'pointer' as const
  }
};

export default GuestGamePage;
