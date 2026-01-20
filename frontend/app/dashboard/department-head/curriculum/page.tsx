'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function CurriculumManagement() {
  const [curriculum, setCurriculum] = useState<any[]>([]);
  const [department, setDepartment] = useState('Computer Science');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchCurriculum();
  }, [department]);

  const fetchCurriculum = async () => {
    try {
      const response = await api.get(`/departmenthead/curriculum?department=${department}`);
      setCurriculum(response.data);
    } catch (error) {
      toast.error('Failed to load curriculum');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout role="DepartmentHead">
        <div>Loading...</div>
      </Layout>
    );
  }

  return (
    <Layout role="DepartmentHead">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Curriculum Management</h1>
        
        <div className="mb-4">
          <label className="block text-sm font-medium mb-2">Department</label>
          <input
            type="text"
            value={department}
            onChange={(e) => setDepartment(e.target.value)}
            className="w-full px-3 py-2 border rounded-lg"
          />
        </div>

        {curriculum.length === 0 ? (
          <p className="text-gray-600">No curriculum data available.</p>
        ) : (
          <div className="space-y-6">
            {curriculum.map((year) => (
              <div key={year.academicYear} className="border rounded-lg p-4">
                <h3 className="font-semibold text-lg mb-4">Academic Year {year.academicYear}</h3>
                <table className="w-full border-collapse border">
                  <thead>
                    <tr className="bg-gray-100">
                      <th className="border p-2">Course Code</th>
                      <th className="border p-2">Course Name</th>
                      <th className="border p-2">Credit Hours</th>
                      <th className="border p-2">Semester</th>
                    </tr>
                  </thead>
                  <tbody>
                    {year.courses.map((course: any, index: number) => (
                      <tr key={index}>
                        <td className="border p-2">{course.courseCode}</td>
                        <td className="border p-2">{course.courseName}</td>
                        <td className="border p-2">{course.creditHours}</td>
                        <td className="border p-2">{course.semester || 'N/A'}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
}
