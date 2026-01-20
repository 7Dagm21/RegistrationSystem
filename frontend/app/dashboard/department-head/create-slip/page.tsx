'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function CreateSlip() {
  const [formData, setFormData] = useState({
    studentID: '',
    semester: '',
    courses: [] as any[],
  });
  const [availableCourses, setAvailableCourses] = useState<any[]>([]);
  const [academicYear, setAcademicYear] = useState(1);
  const [department, setDepartment] = useState('');

  useEffect(() => {
    // Get user's department (simplified - in real app, get from auth)
    setDepartment('Computer Science');
  }, []);

  const fetchCourses = async () => {
    if (!academicYear || !department) return;
    try {
      const response = await api.get(`/departmenthead/courses?academicYear=${academicYear}&department=${department}`);
      setAvailableCourses(response.data);
    } catch (error) {
      toast.error('Failed to load courses');
    }
  };

  useEffect(() => {
    fetchCourses();
  }, [academicYear, department]);

  const handleAddCourse = (course: any) => {
    if (!formData.courses.find((c) => c.courseCode === course.courseCode)) {
      setFormData({
        ...formData,
        courses: [...formData.courses, course],
      });
    }
  };

  const handleRemoveCourse = (courseCode: string) => {
    setFormData({
      ...formData,
      courses: formData.courses.filter((c) => c.courseCode !== courseCode),
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.studentID || !formData.semester || formData.courses.length === 0) {
      toast.error('Please fill all fields');
      return;
    }

    try {
      const response = await api.post('/departmenthead/slips/create', formData);
      toast.success('Slip created successfully');
      setFormData({ studentID: '', semester: '', courses: [] });
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Failed to create slip');
    }
  };

  return (
    <Layout role="DepartmentHead">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Create Registration Slip</h1>
        
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-2">Student ID</label>
            <input
              type="text"
              value={formData.studentID}
              onChange={(e) => setFormData({ ...formData, studentID: e.target.value })}
              className="w-full px-3 py-2 border rounded-lg"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Semester</label>
            <input
              type="text"
              value={formData.semester}
              onChange={(e) => setFormData({ ...formData, semester: e.target.value })}
              className="w-full px-3 py-2 border rounded-lg"
              placeholder="e.g., 2024-2025 Fall"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Academic Year</label>
            <select
              value={academicYear}
              onChange={(e) => setAcademicYear(parseInt(e.target.value))}
              className="w-full px-3 py-2 border rounded-lg"
            >
              <option value={1}>Year 1</option>
              <option value={2}>Year 2</option>
              <option value={3}>Year 3</option>
              <option value={4}>Year 4</option>
              <option value={5}>Year 5</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Available Courses</label>
            <div className="border rounded-lg p-4 max-h-60 overflow-y-auto">
              {availableCourses.length === 0 ? (
                <p className="text-gray-600">No courses available</p>
              ) : (
                <div className="space-y-2">
                  {availableCourses.map((course) => (
                    <div key={course.courseCode} className="flex justify-between items-center p-2 hover:bg-gray-50">
                      <span>{course.courseCode} - {course.courseName} ({course.creditHours} CH)</span>
                      <button
                        type="button"
                        onClick={() => handleAddCourse(course)}
                        className="text-blue-600 hover:underline"
                      >
                        Add
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Selected Courses</label>
            {formData.courses.length === 0 ? (
              <p className="text-gray-600">No courses selected</p>
            ) : (
              <div className="border rounded-lg p-4">
                <table className="w-full">
                  <thead>
                    <tr className="bg-gray-100">
                      <th className="p-2 text-left">Course Code</th>
                      <th className="p-2 text-left">Course Name</th>
                      <th className="p-2 text-left">Credit Hours</th>
                      <th className="p-2"></th>
                    </tr>
                  </thead>
                  <tbody>
                    {formData.courses.map((course) => (
                      <tr key={course.courseCode}>
                        <td className="p-2">{course.courseCode}</td>
                        <td className="p-2">{course.courseName}</td>
                        <td className="p-2">{course.creditHours}</td>
                        <td className="p-2">
                          <button
                            type="button"
                            onClick={() => handleRemoveCourse(course.courseCode)}
                            className="text-red-600 hover:underline"
                          >
                            Remove
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
                <p className="mt-2">
                  <strong>Total Credit Hours:</strong> {formData.courses.reduce((sum, c) => sum + c.creditHours, 0)}
                </p>
              </div>
            )}
          </div>

          <button
            type="submit"
            className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700"
          >
            Create Slip
          </button>
        </form>
      </div>
    </Layout>
  );
}
