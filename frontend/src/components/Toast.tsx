import React, { useEffect } from 'react';
import '@/components/styles/toast.css';

interface ToastProps {
  message: string;
  type: ToastType;
  onClose: () => void;
};

export enum ToastType {
  Error,
  Information
};

const Toast: React.FC<ToastProps> = ({ message, type, onClose }:ToastProps) => {
  useEffect(() => {
    const timer = setTimeout(onClose, 3000); // Auto-dismiss after 3s
    return () => clearTimeout(timer);
  }, [onClose]);

  const getToastStyling = () => {
    switch(type) {
      case ToastType.Error:
        return 'toast-error';
      case ToastType.Information:
        return 'toast-information';
    }
  };

  return <div className={getToastStyling()}>{message}</div>;
};

export default Toast;
