
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Blog from './components/Blog/Blog';
import BlogDetails from './components/Blog/BlogDetails';

const App = () => {
  return (
    <Router>
      <Switch>
        <Route exact path="/" component={Blog} />
        <Route path="/post/:id" component={BlogDetails} />
      </Switch>
    </Router>
  );
};

export default App;