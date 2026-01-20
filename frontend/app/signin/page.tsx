'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import api from '@/lib/api';
import { setAuth } from '@/lib/auth';
import { toast } from 'react-toastify';

export default function SignInPage() {
  const router = useRouter();
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });

  const handleSignIn = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const response = await api.post('/auth/signin', formData);
      const authData = response.data;
      
      // Normalize and save auth data
      setAuth(authData);
      
      // Get role (handle both camelCase and PascalCase for safety)
      const role = authData.role || authData.Role;
      
      console.log('Sign in successful, role:', role, 'Full response:', authData);
      
      if (!role) {
        toast.error('No role found in response');
        console.error('Auth response:', authData);
        return;
      }
      
      // Map role to dashboard path
      const rolePathMap: Record<string, string> = {
        'Student': '/dashboard/student',
        'Advisor': '/dashboard/advisor',
        'DepartmentHead': '/dashboard/department-head',
        'Registrar': '/dashboard/registrar',
        'CostSharingOfficer': '/dashboard/cost-sharing',
        'SystemAdmin': '/dashboard/admin'
      };
      
      const redirectPath = rolePathMap[role];
      
      if (!redirectPath) {
        toast.warning(`Unknown role: ${role}. Redirecting to student dashboard.`);
        router.push('/dashboard/student');
        return;
      }
      
      toast.success('Signed in successfully');
      
      // Small delay to ensure toast is visible, then redirect
      setTimeout(() => {
        router.push(redirectPath);
        router.refresh(); // Force refresh to ensure auth state is updated
      }, 300);
    } catch (error: any) {
      console.error('Sign in error:', error);
      toast.error(error.response?.data?.message || 'Sign in failed');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="max-w-md w-full bg-white p-8 rounded-lg shadow">
        <h2 className="text-2xl font-bold mb-6">Sign In</h2>
        <form onSubmit={handleSignIn}>
          <div className="mb-4">
            <label className="block text-sm font-medium mb-2">Email</label>
            <input
              type="email"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              className="w-full px-3 py-2 border rounded-lg"
              required
            />
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium mb-2">Password</label>
            <input
              type="password"
              value={formData.password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value })}
              className="w-full px-3 py-2 border rounded-lg"
              required
            />
          </div>
          <button
            type="submit"
            className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700"
          >
            Sign In
          </button>
        </form>
        <p className="mt-4 text-sm text-gray-600 text-center">
          Don't have an account? <Link href="/signup" className="text-blue-600">Sign Up</Link>
        </p>
      </div>
    </div>
  );
}
