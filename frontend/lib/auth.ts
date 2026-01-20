import Cookies from 'js-cookie';

export interface User {
  token: string;
  role: string;
  userId: string;
  fullName: string;
  email: string;
}

export const setAuth = (user: User | any) => {
  // Normalize user object to ensure consistent format
  const normalizedUser: User = {
    token: user.token || user.Token || '',
    role: user.role || user.Role || '',
    userId: user.userId || user.UserId || '',
    fullName: user.fullName || user.FullName || '',
    email: user.email || user.Email || ''
  };
  
  Cookies.set('token', normalizedUser.token, { expires: 7 });
  Cookies.set('user', JSON.stringify(normalizedUser), { expires: 7 });
};

export const getAuth = (): User | null => {
  const userStr = Cookies.get('user');
  if (!userStr) return null;
  return JSON.parse(userStr);
};

export const clearAuth = () => {
  Cookies.remove('token');
  Cookies.remove('user');
};

export const getRole = (): string | null => {
  const user = getAuth();
  return user?.role || null;
};
