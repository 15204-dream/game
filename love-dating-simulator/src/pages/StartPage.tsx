import React from 'react';
import { useNavigate } from 'react-router-dom';

const StartPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <div style={styles.container}>
      <h1 style={styles.title}>糟糕！是心动鸭！</h1>
      <p style={styles.subtitle}>一款像素风恋综模拟器</p>
      <div style={styles.buttonContainer}>
        <button style={styles.button} onClick={() => navigate('/select-role')}>
          开始游戏
        </button>
        <button style={styles.buttonSecondary} onClick={() => console.log('继续游戏')}>
          继续游戏
        </button>
      </div>
    </div>
  );
};

const styles = {
  container: {
    display: 'flex' as const,
    flexDirection: 'column' as const,
    alignItems: 'center' as const,
    justifyContent: 'center' as const,
    minHeight: '100vh',
    backgroundColor: '#FFE4E1',
    textAlign: 'center' as const,
    padding: '20px'
  },
  title: {
    fontSize: '48px',
    color: '#FF6B6B',
    marginBottom: '20px',
    textShadow: '3px 3px 0 #fff'
  },
  subtitle: {
    fontSize: '20px',
    color: '#666',
    marginBottom: '40px'
  },
  buttonContainer: {
    display: 'flex' as const,
    flexDirection: 'column' as const,
    gap: '16px',
    width: '200px'
  },
  button: {
    padding: '16px 32px',
    fontSize: '18px',
    backgroundColor: '#FF6B6B',
    color: 'white',
    border: 'none',
    borderRadius: '8px',
    cursor: 'pointer' as const,
    transition: 'transform 0.2s'
  },
  buttonSecondary: {
    padding: '16px 32px',
    fontSize: '18px',
    backgroundColor: '#fff',
    color: '#FF6B6B',
    border: '2px solid #FF6B6B',
    borderRadius: '8px',
    cursor: 'pointer' as const,
    transition: 'transform 0.2s'
  }
};

export default StartPage;
