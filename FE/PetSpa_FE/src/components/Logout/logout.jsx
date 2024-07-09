import { useNavigate } from 'react-router-dom';

const Logout = () => {
  const navigate = useNavigate();
  const handleLogout = () => {
    // Assuming you clear user info from localStorage or session storage
    localStorage.removeItem("user-info");
    // Redirect to login page after logout
    navigate('/login');
  };

  return (
    <div className="flex items-center justify-center h-screen">
      <div className="bg-white p-8 rounded-lg shadow-md max-w-md w-full">
        <h2 className="text-3xl font-semibold mb-6 text-center">Logout</h2>
        <p className="mb-4 text-center">Are you sure you want to logout?</p>
        <div className="flex justify-center">
          <button
            onClick={handleLogout}
            className="bg-red-500 hover:bg-red-600 text-white font-bold py-2 px-4 rounded mr-2"
          >
            Logout
          </button>
          <button
            onClick={() => window.history.back()}
            className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold py-2 px-4 rounded"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
};

export default Logout;
