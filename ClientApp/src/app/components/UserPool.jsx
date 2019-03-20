import React, { Component } from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import { DebounceInput } from 'react-debounce-input';
import { UserCard } from './Profile/UserCard';
import { Loader } from './Loader';

export class UserPool extends Component {
  _isMounted = false;

  constructor(props) {
    super(props);
    this.state = {
      loader: true
    };
    this.filterList = this.filterList.bind(this);
  }

  async componentDidMount() {
    this._isMounted = true;
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/user`)
      .then(res => res.json())
      .then((res) => {
        if (this._isMounted) {
          this.setState({
            initialUsers: res,
            users: res,
            loader: false
          });
        }
      });
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  filterList(e) {
    const { initialUsers } = this.state;
    if (initialUsers) {
      let updatedList = initialUsers;
      updatedList = _.filter(updatedList, user => user.userName.toLowerCase()
        .search(e.target.value.toLowerCase()) !== -1);
      if (this._isMounted) {
        this.setState({ users: updatedList });
      }
    }
  }

  render() {
    const { loader, users } = this.state;
    if (loader && !this._isMounted) { return <div className="m-5"><Loader show={loader} /></div>; }


    const userCards = _.map(
      users,
      user => (
        <UserCard
          key={shortid()}
          user={user}
          color={user.color}
        />
      )
    );
    return (
      <div className="container bg-light">
        <div className="search m-3 mb-4">
          <span className="fa fa-search" />
          <DebounceInput
            className="UserPool__Search form-control"
            type="search"
            name="search"
            placeholder="Search..."
            debounceTimeout={600}
            onChange={this.filterList}
          />
        </div>
        <div className="center col">
          <div className="row">
            {userCards}
          </div>
        </div>
      </div>
    );
  }
}

export default UserPool;
