declare module Heroic
{
    class Session { userId: string; }
    class User { id: string; }
    type UserOrSession = User | Session;

    module Server
    {
        function currentTimeMillis(): number;
    }

    module Logger
    {
        function info(message: string): void;
        function warn(message: string): void;
        function error(message: string): void;
    }

    module Script
    {
        function mode(): ('direct' | 'timed' | 'pre' | 'post');
    }

    module Request
    {
        function currentSession(): Session;
        function body(): any;
    }

    module Response
    {
        class Response { }

        function success(obj?: any): Response;
        function error(status: number, reason: string): Response;
    }

    module User
    {
        function findBySession(session: Session): Promise<User>;
        function findByNickname(nicknames: (string | string[])): Promise<User[]>;
    }

    module Storage
    {
        module Cloud
        {
            function read(userOrSession: UserOrSession, key: string): Promise<any>;
            function write(userOrSession: UserOrSession, key: string, obj: any): Promise<void>;
            function remove(userOrSession: UserOrSession, key: string): Promise<void>;
        }

        module Shared
        {
            class SharedStorageObject { public: any; protected: any }
            class QueryResponse { total_count: number; results: SharedStorageObject[]; }

            function query(query: string, key?: string, sortField?: string, limit?: number, offset?: number): Promise<QueryResponse>;
            function read(userOrSession: UserOrSession, key: string): Promise<SharedStorageObject>;
            function writePublic(userOrSession: UserOrSession, key: string, obj: any): Promise<void>;
            function writeProtected(userOrSession: UserOrSession, key: string, obj: any): Promise<void>;
            function updatePublic(userOrSession: UserOrSession, key: string, obj: any): Promise<void>;
            function updateProtected(userOrSession: UserOrSession, key: string, obj: any): Promise<void>;
            function remove(userOrSession: UserOrSession, key: string): Promise<void>;
        }
    }

    module Mailbox
    {
        class MailboxMessage {
            message_id: string,
            subject: string
            created_at: number,
            expires_at?: number,
            read_at?: number,
            tags?: string[],
            body?: any;
        }

        class MailboxMessageList {
            count: number,
            message: MailboxMessage
        }

        function getMessages(userOrSession: UserOrSession, includeBody?: boolean, timestamp?: number): Promise<MailboxMessageList>;
        function read(userOrSession: UserOrSession, message: MailboxMessage, includeBody?: boolean): Promise<MailboxMessage>;
        function remove(userOrSession: UserOrSession, message: MailboxMessage): Promise<MailboxMessage>;
        function send(userOrSession: UserOrSession, subject: string, body: string, tags?: string[], expiry?: number): Promise<MailboxMessage>;
    }

    module Http
    {
        module Request
        {
            function create(url: string): HTTPRequest;
        }

        class HTTPRequest
        {
            setMethod(method: ('GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE')): HTTPRequest;
            addHeader(name: string, value: string): HTTPRequest;
            setBody(body: string): HTTPRequest;
            execute(): Promise<HTTPResponse>;
        }

        class HTTPResponse
        {
            status: number;
            body: string;
            headers: [{ [name: string]: string[]; }];
        }
    }
}
