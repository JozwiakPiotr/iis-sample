import React, { useEffect, useState } from 'react'

export default function App(){
  const [users, setUsers] = useState([])
  const [name, setName] = useState('')
  const [backendAvailable, setBackendAvailable] = useState(null)

  useEffect(()=>{
    fetch('/api/users')
      .then(r=>{
        if(!r.ok) throw new Error('Backend request failed')
        return r.json()
      })
      .then(data=>{
        setUsers(data)
        setBackendAvailable(true)
      })
      .catch(()=>setBackendAvailable(false))
  }, [])

  const add = async ()=>{
    const r = await fetch('/api/users', {method:'POST', headers:{'Content-Type':'application/json'}, body: JSON.stringify({ name })})
    if(r.ok){ setName(''); setUsers(await (await fetch('/api/users')).json()) }
  }

  const del = async (id)=>{ await fetch('/api/users/'+id, {method:'DELETE'}); setUsers(users.filter(u=>u.id!==id)) }

  return (
    <div style={{padding:20}}>
      <h1>Hello World</h1>
      <p>Frontend działa.</p>
      {backendAvailable === false && <p>Backend jest obecnie niedostępny. Możesz wrócić później.</p>}
      <h2>Users</h2>
      <input value={name} onChange={e=>setName(e.target.value)} placeholder="Name" disabled={backendAvailable !== true} />
      <button onClick={add} disabled={backendAvailable !== true}>Add</button>
      <ul>
        {users.map(u=> <li key={u.id}>{u.name} <button onClick={()=>del(u.id)}>Delete</button></li>)}
      </ul>
    </div>
  )
}
