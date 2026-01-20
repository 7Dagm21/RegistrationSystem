'use client';

import { useEffect } from 'react';
import { useRouter, usePathname } from 'next/navigation';
import Link from 'next/link';
import { getAuth, clearAuth } from '@/lib/auth';

interface LayoutProps {
  children: React.ReactNode;
  role: string;
}

export default function Layout({ children, role }: LayoutProps) {
  const router = useRouter();
  const pathname = usePathname();
  const user = getAuth();

  useEffect(() => {
    if (!user) {
      router.push('/signin');
      return;
    }
    const normalizedRole = normalizeRole(role);
    if (user.role !== normalizedRole && user.role !== role) {
      router.push('/signin');
    }
  }, [user, role, router]);

  const handleLogout = () => {
    clearAuth();
    router.push('/');
  };

  if (!user) {
    return null;
  }

  const normalizedRole = normalizeRole(role);
  const userRole = user.role;
  
  // Check if user role matches the required role (normalized)
  if (userRole !== normalizedRole && userRole !== role) {
    return null;
  }

  const navItems = getNavItems(userRole);

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow">
        <div className="container mx-auto px-4">
          <div className="flex justify-between items-center py-4">
            <div className="flex items-center space-x-8">
              <Link href={`/dashboard/${role.toLowerCase().replace(' ', '-')}`} className="text-xl font-bold">
                AASTU Registration
              </Link>
              <div className="flex space-x-4">
                {navItems.map((item) => (
                  <Link
                    key={item.href}
                    href={item.href}
                    className={`px-3 py-2 rounded ${
                      pathname === item.href
                        ? 'bg-blue-600 text-white'
                        : 'text-gray-700 hover:bg-gray-100'
                    }`}
                  >
                    {item.label}
                  </Link>
                ))}
              </div>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-sm text-gray-600">{user.fullName}</span>
              <button
                onClick={handleLogout}
                className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>
      <main className="container mx-auto px-4 py-8">{children}</main>
    </div>
  );
}

function getNavItems(role: string) {
  const roleMap: Record<string, Array<{ href: string; label: string }>> = {
    Student: [
      { href: '/dashboard/student', label: 'Home' },
      { href: '/dashboard/student/slips', label: 'My Slips' },
      { href: '/dashboard/student/cost-sharing', label: 'Cost Sharing' },
      { href: '/dashboard/student/grades', label: 'Grade Reports' },
    ],
    Advisor: [
      { href: '/dashboard/advisor', label: 'Home' },
      { href: '/dashboard/advisor/create-slip', label: 'Create Slip' },
      { href: '/dashboard/advisor/pending', label: 'Pending Approvals' },
    ],
    DepartmentHead: [
      { href: '/dashboard/department-head', label: 'Home' },
      { href: '/dashboard/department-head/search', label: 'Search Student' },
      { href: '/dashboard/department-head/create-slip', label: 'Create Slip' },
      { href: '/dashboard/department-head/grade-reports', label: 'Grade Reports' },
      { href: '/dashboard/department-head/curriculum', label: 'Curriculum' },
    ],
    Registrar: [
      { href: '/dashboard/registrar', label: 'Home' },
      { href: '/dashboard/registrar/pending', label: 'Pending Approvals' },
      { href: '/dashboard/registrar/create-grade-report', label: 'Create Grade Report' },
      { href: '/dashboard/registrar/archive', label: 'Archive' },
    ],
    CostSharingOfficer: [
      { href: '/dashboard/cost-sharing', label: 'Home' },
      { href: '/dashboard/cost-sharing/pending', label: 'Pending Payments' },
    ],
    SystemAdmin: [
      { href: '/dashboard/admin', label: 'Home' },
      { href: '/dashboard/admin/students', label: 'Students' },
      { href: '/dashboard/admin/staff', label: 'Staff' },
      { href: '/dashboard/admin/users', label: 'Users' },
      { href: '/dashboard/admin/audit-logs', label: 'Audit Logs' },
    ],
  };

  return roleMap[role] || [];
}

function normalizeRole(role: string): string {
  const roleMap: Record<string, string> = {
    'student': 'Student',
    'advisor': 'Advisor',
    'department-head': 'DepartmentHead',
    'departmenthead': 'DepartmentHead',
    'registrar': 'Registrar',
    'cost-sharing': 'CostSharingOfficer',
    'costsharingofficer': 'CostSharingOfficer',
    'admin': 'SystemAdmin',
    'systemadmin': 'SystemAdmin',
  };
  return roleMap[role.toLowerCase()] || role;
}
