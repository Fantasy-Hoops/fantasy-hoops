import React, { Component } from 'react';
import _ from 'lodash';
import shortid from 'shortid';
import InfiniteScroll from 'react-infinite-scroll-component';
import { NewsCard } from './NewsCard';
import { Loader } from '../Loader';

export class NewsFeed extends Component {
  _isMounted = false;

  constructor(props) {
    super(props);
    this.state = {
      news: '',
      hasMore: true,
      newsLoader: false
    };
    this.fetchData = this.fetchData.bind(this);
  }

  componentDidMount() {
    this._isMounted = true;

    this.setState({
      newsLoader: true
    });
    fetch(`${process.env.REACT_APP_SERVER_NAME}/api/news`)
      .then(res => res.json())
      .then((res) => {
        if (this._isMounted) {
          this.setState({
            news: res,
            newsLoader: false
          });
        }
      });
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  fetchData() {
    const { news } = this.state;
    if (this._isMounted) {
      this.setState({
        newsLoader: true
      });
    }

    fetch(`${process.env.REACT_APP_SERVER_NAME}/api/news?start=${news.length}`)
      .then(res => res.json())
      .then((res) => {
        if (this._isMounted) {
          this.setState({
            news: news.concat(res),
            hasMore: res.length === 6,
            newsLoader: false
          });
        }
      });
  }

  render() {
    const { news, hasMore, newsLoader } = this.state;
    const newsCards = _.map(news,
      newsObj => (
        <NewsCard
          key={shortid()}
          news={newsObj}
        />
      ));
    return (
      <div className="container bg-light pt-5">
        <div className="center col">
          <InfiniteScroll
            dataLength={news.length}
            next={this.fetchData}
            hasMore={hasMore}
            loader={<Loader show={newsLoader} />}
          >
            {newsCards}
          </InfiniteScroll>
        </div>
      </div>
    );
  }
}

export default NewsFeed;
