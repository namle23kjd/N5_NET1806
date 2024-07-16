import { useState, useEffect } from "react";
import axios from "axios";
import "../../assets/css/StaffPage.css";
import { useNavigate } from "react-router-dom";
import { message } from "antd";

const StaffPage = () => {
  const [tasks, setTasks] = useState({
    todo: [],
    inProgress: [],
    done: [],
    cancel: [],
  });
  const [activeTab, setActiveTab] = useState("todo");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    const fetchData = async () => {
      try {
        const userInfoString = localStorage.getItem("user-info");
        if (!userInfoString) {
          navigate("/login");
          return;
        }

        const userInfo = JSON.parse(userInfoString);
        const token = userInfo.data.token;
        const userId = userInfo.data.user.id;

        const headers = {
          Authorization: `Bearer ${token}`,
        };

        const todoResponse = await axios.get(
          `https://localhost:7150/api/Staff/${userId}/pending-bookings`,
          { headers }
        );
        console.log("Todo Response:", todoResponse.data);

        const inProgressResponse = await axios.get(
          `https://localhost:7150/api/Staff/${userId}/current-booking`,
          { headers }
        );
        console.log("In Progress Response:", inProgressResponse.data);

        const doneResponse = await axios.get(
          `https://localhost:7150/api/Staff/${userId}/completed-bookings`,
          { headers }
        );
        console.log("Done Response:", doneResponse.data);

        const cancelResponse = await axios.get(
          `https://localhost:7150/api/Booking/staff-denied-bookings/${userId}`,
          { headers }
        );
        console.log("Cancel Response:", cancelResponse.data.data.bookings);

        setTasks({
          todo: Array.isArray(todoResponse.data)
            ? mapApiResponse(todoResponse.data)
            : [],
          inProgress: Array.isArray(inProgressResponse.data)
            ? mapApiResponse(inProgressResponse.data)
            : [],
          done: Array.isArray(doneResponse.data)
            ? mapApiResponse(doneResponse.data)
            : [],
          cancel: Array.isArray(cancelResponse.data.data.bookings)
            ? mapApiResponse(cancelResponse.data.data.bookings)
            : [],
        });
        console.log(tasks.cancel);
      } catch (error) {
        if (error.response && error.response.status === 403) {
          setError("You are not authorized to access this resource.");
        } else {
          console.error("Error fetching data:", error);
        }
      }
    };

    fetchData();
  }, [navigate]);

  const mapApiResponse = (data) => {
    return data.map((item) => ({
      id: item.bookingId,
      service: item.serviceName,
      pet: item.petName,
      owner: item.customerName,
      date: new Date(item.startDate).toLocaleDateString(),
      time: new Date(item.startDate).toLocaleTimeString(),
    }));
  };

  const handleDeny = async (taskId) => {
    try {
      await axios.put(`https://localhost:7150/api/Booking/deny-booking`, {
        bookingId: taskId,
      });

      setTasks((prevTasks) => {
        const taskToDeny =
          prevTasks.todo.find((task) => task.id === taskId) ||
          prevTasks.inProgress.find((task) => task.id === taskId) ||
          prevTasks.done.find((task) => task.id === taskId);
        return {
          ...prevTasks,
          todo: prevTasks.todo.filter((task) => task.id !== taskId),
          inProgress: prevTasks.inProgress.filter((task) => task.id !== taskId),
          done: prevTasks.done.filter((task) => task.id !== taskId),
          cancel: [...prevTasks.cancel, taskToDeny],
        };
      });
      message.success("Update status booking successfully");
    } catch (error) {
      console.error("Error denying task:", error);
    }
  };

  const printTasks = (tasksToPrint) => {
    const newWindow = window.open("", "", "height=800,width=600");
    newWindow.document.write("<html><head><title>Print</title>");
    newWindow.document.write(
      '<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" type="text/css" />'
    );
    newWindow.document.write("<style>");
    newWindow.document.write(
      "@media print { body { -webkit-print-color-adjust: exact; margin: 20px; font-size: 18px; }"
    );
    newWindow.document.write(
      ".printable-content { width: 100%; margin: auto; }"
    );
    newWindow.document.write(
      ".card-body { padding: 20px; border: 1px solid #dee2e6; border-radius: 4px; }"
    );
    newWindow.document.write(
      ".media-1 { margin-bottom: 20px; display: flex; align-items: center; }"
    );
    newWindow.document.write(".media-body { padding: 10px; }");
    newWindow.document.write("img { max-width: 100%; height: auto; }");
    newWindow.document.write(
      ".h5, .h5 a { font-size: 1.5rem; font-weight: 500; margin-bottom: 0.5rem; }"
    );
    newWindow.document.write(".text-body { font-size: 1rem; color: #000; }");
    newWindow.document.write(".d-print-none { display: none; }"); // Hide print button
    newWindow.document.write("</style>");
    newWindow.document.write("</head><body class='printable-content'>");

    tasksToPrint.forEach((task) => {
      newWindow.document.write(
        `<div class='card-body'>${task.service} for ${task.pet} (${task.id}) at ${task.time} on ${task.date}</div>`
      );
      newWindow.document.write("<hr>"); // Add a separator between tasks
    });

    newWindow.document.close();
    newWindow.focus(); // Necessary for IE >= 10
    newWindow.print();
  };

  return (
    <div className="staff-page">
      <h1 className="hehe">Staff Page</h1>
      {error && <div className="error-message">{error}</div>}
      <div className="tab-section">
        <div
          className={`tab-title ${activeTab === "todo" ? "active" : ""}`}
          onClick={() => setActiveTab("todo")}
        >
          To Do ({tasks.todo.length})
        </div>
        <div
          className={`tab-title ${activeTab === "inProgress" ? "active" : ""}`}
          onClick={() => setActiveTab("inProgress")}
        >
          In Progress ({tasks.inProgress.length})
        </div>
        <div
          className={`tab-title ${activeTab === "done" ? "active" : ""}`}
          onClick={() => setActiveTab("done")}
        >
          Completed ({tasks.done.length})
        </div>
        <div
          className={`tab-title ${activeTab === "cancel" ? "active" : ""}`}
          onClick={() => setActiveTab("cancel")}
        >
          Canceled ({tasks.cancel.length})
        </div>
      </div>
      <div className={`task-content ${activeTab !== "todo" ? "hidden" : ""}`}>
        <TaskList
          tasks={tasks.todo}
          printTasks={printTasks}
          handleDeny={handleDeny}
        />
      </div>
      <div
        className={`task-content ${activeTab !== "inProgress" ? "hidden" : ""}`}
      >
        <TaskList
          tasks={tasks.inProgress}
          printTasks={printTasks}
          handleDeny={handleDeny}
        />
      </div>
      <div className={`task-content ${activeTab !== "done" ? "hidden" : ""}`}>
        <TaskList
          tasks={tasks.done}
          printTasks={printTasks}
          handleDeny={handleDeny}
          isDoneTab
        />
      </div>
      <div className={`task-content ${activeTab !== "cancel" ? "hidden" : ""}`}>
        <TaskList tasks={tasks.cancel} printTasks={printTasks} isCancelTab />
      </div>
    </div>
  );
};

const TaskList = ({
  tasks,
  printTasks,
  handleDeny,
  isDoneTab = false,
  isCancelTab = false,
}) => {
  const [selectedTasks, setSelectedTasks] = useState([]);

  const handleTaskSelect = (task) => {
    setSelectedTasks((prevSelectedTasks) =>
      prevSelectedTasks.includes(task)
        ? prevSelectedTasks.filter((t) => t !== task)
        : [...prevSelectedTasks, task]
    );
  };

  return (
    <div>
      <table>
        <thead>
          <tr>
            <th>#</th>
            <th>Service</th>
            <th>Pet</th>
            <th>Owner</th>
            <th>Date</th>
            <th>Time</th>
            <th>Select</th>
            {isDoneTab && !isCancelTab && <th>Action</th>}
          </tr>
        </thead>
        <tbody>
          {tasks.map((task, index) => (
            <tr key={task.id}>
              <td>{index + 1}</td>
              <td>{task.service}</td>
              <td>{task.pet}</td>
              <td>{task.owner}</td>
              <td>{task.date}</td>
              <td>{task.time}</td>
              <td>
                <input
                  type="checkbox"
                  checked={selectedTasks.includes(task)}
                  onChange={() => handleTaskSelect(task)}
                />
              </td>
              {isDoneTab && !isCancelTab && (
                <td>
                  <button onClick={() => handleDeny(task.id)}>Deny</button>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
      <button
        className="print-button"
        onClick={() => printTasks(selectedTasks)}
        disabled={selectedTasks.length === 0}
      >
        Print Selected Tasks
      </button>
    </div>
  );
};

export default StaffPage;
