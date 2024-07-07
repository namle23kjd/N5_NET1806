import { useParams } from "react-router-dom";
import "./Blog_Details.css";

const BlogDetails = () => {
  const { id } = useParams();
  const post = JSON.parse(localStorage.getItem("selectedPost"));

  if (!post || post.id !== parseInt(id)) {
    return <p>Post not found.</p>;
  }

  return (
    <div className="blog-details-container">
      <div className="blog-header">
        <img
          src={post.imageUrl}
          alt={post.title}
          className="blog-header-image"
        />
        <h1 className="blog-title">{post.title}</h1>
        <p className="blog-meta">
          By {post.author} | {post.date}
        </p>
      </div>
      <div className="blog-content">
        <p>{post.content}</p>
      </div>
    </div>
  );
};

export default BlogDetails;
