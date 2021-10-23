import * as React from 'react';
import {Route} from 'react-router-dom';
import {Redirect} from 'react-router-dom';
import {isAuth} from '../../utils/auth';

export default function PrivateRoute({children, ...rest}) {
    return <Route
        {...rest}
        render={props => (
            isAuth()
                ? children
                : (
                    <Redirect to={{
                        pathname: '/login',
                        state: {
                            error: 'You must login to proceed!',
                            fallback: this ? props.location.pathname : '/'
                        }
                    }}
                    />
                )
        )}
    />
}

// use only for class based components, as they do not support hooks.
// it's better to rewrite components to functional ones, but \_(ツ)_/¯
export function ClassBasedPrivateRoute({ component: Component, ...rest }) {
    return (
        <Route
            {...rest}
            render={props => (
                isAuth()
                    ? <Component {...props} />
                    : (
                        <Redirect to={{
                            pathname: '/login',
                            state: {
                                error: 'You must login to proceed!',
                                fallback: this ? props.location.pathname : '/'
                            }
                        }}
                        />
                    )
            )}
        />
    )
}
