
import { Link } from 'react-router-dom';
import './Blog.css';

const blogPosts = [
  {
    id: 1,
    title: "The Benefits of a Regular Pet Spa Day",
    date: "06.15.2023",
    author: "Crasstella Curtis",
    imageUrl: "/src/components/Blog/2.jpg", // replace with actual image path
    excerpt: "Pampering your pet with a regular spa day can...",
    content: "Pampering your pet with a regular spa day can significantly improve their health and happiness. Grooming services, including baths, haircuts, and nail trimming, ensure your pet looks and feels their best. Massage therapy relieves stress and improves circulation, particularly beneficial for older pets or those with joint issues. Hydrotherapy helps with rehabilitation, while aromatherapy can calm anxious pets. Dental care services maintain oral hygiene, and specialty treatments like blueberry facials and pawdicures add a luxurious touch. Regular spa visits enhance bonding, reduce stress, and prevent health issues. Choose a reputable spa with trained professionals for the best experience.",
  },
  {
    id: 2,
    title: "Top Pet Spa Treatments Your Furry Friend Will Love",
    date: "06.20.2023",
    author: "Jordan Smith",
    imageUrl: "/src/components/Blog/3.jpg",
    excerpt: "Discover the top pet spa treatments that will...",
    content: "Discover the top pet spa treatments that will leave your furry friend feeling pampered and rejuvenated. Grooming services such as baths, haircuts, and ear cleaning use high-quality products tailored to your pet's needs. Massage therapy promotes relaxation and stress relief, ideal for pets with anxiety or arthritis. Hydrotherapy provides low-impact exercise for rehabilitation. Aromatherapy with pet-safe essential oils enhances well-being. Dental care services ensure a healthy smile, while specialty treatments like coat conditioning and pawdicures offer a luxurious experience. Treat your pet to these spa services for a happier, healthier life.",
  },
  // Add more blog posts here
];

const Blog = () => {
  const handlePostClick = (post) => {
    localStorage.setItem('selectedPost', JSON.stringify(post));
  };

  return (
    <div className="blog-container">
      <h1>Our Blog</h1>
      
      <div className="blog-content">
        <div className="blog-posts">
          {blogPosts.map((post) => (
            <div key={post.id} className="blog-post">
              <img src={post.imageUrl} alt={post.title} />
              <div className="post-content">
                <h2>{post.title}</h2>
                <p>By {post.author} | {post.date}</p>
                <p>{post.excerpt}</p>
                <Link to={`/post/${post.id}`} onClick={() => handlePostClick(post)}>
                  Read Post
                </Link>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default Blog;